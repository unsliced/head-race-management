using System;
using DropBoxSync.iOS;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using TimingApp.Data.Factories;

namespace TimingApp_iOS.DropboxBoat 
{
	public class DropboxDatabase : IRepository
	{
		const string DatastoreId = ".8K0ATSTNNGqlZvqwFi7HQm_Fd6oA9tbLT3ZPPWbGKHg";

		IDictionary<string,DBRecord> _raceRecords = new Dictionary<string, DBRecord> ();
		IDictionary<string,DropboxRace> _raceDictionary;

		IDictionary<int, DBRecord> _boatRecords = new Dictionary<int, DBRecord>();
		IDictionary<int, IBoat> _boatDictionary;

		// where Tuple<string,string,int> represents <LocationName, LocationToken, StartNumber> 
		IDictionary<Tuple<string,string,int>, DBRecord> _sequenceRecords = new Dictionary<Tuple<string, string, int>, DBRecord>();
		IDictionary<Tuple<string,string,int>, ISequenceItem> _sequenceDictionary;

		DropboxRace _race = null;

		static DropboxDatabase shared;

		public bool AutoUpdating { get; set; }

		public static DropboxDatabase Shared {
			get {
				if (shared == null)
					shared = new DropboxDatabase ();
				return shared;
			}
		}

		public event EventHandler RaceListUpdated;
		// todo - could definitely use this carrying some more information 
		public event EventHandler ItemsListUpdated;

		DBDatastore _generalStore;
		DBDatastore _raceStore;


		public DropboxDatabase ()
		{
			_raceDictionary = new Dictionary<string, DropboxRace>();
			_boatDictionary = new Dictionary<int, IBoat>();
			_sequenceDictionary = new Dictionary<Tuple<string,string,int>,ISequenceItem>();
		}

		public void Init ()
		{
			if (_generalStore != null)
				return;
			DBError error;
			var manager = DBDatastoreManager.Manager(DBAccountManager.SharedManager.LinkedAccount);
			if(string.IsNullOrEmpty(DatastoreId))
			{
				_generalStore = manager.CreateDatastore(out error);
				Debug.WriteLine("Datastore created, take a note of this:", _generalStore.DatastoreId);
				_generalStore.SetRole("public", DBRole.Editor);
			}
			else
				_generalStore = manager.OpenDatastore(DatastoreId, out error);

			_generalStore.Sync (out error);

			// DeleteAll();
			_generalStore.Sync (out error);

			_generalStore.AddObserver (_generalStore, () => {
				LoadData ();
			});
			AutoUpdating = true;
		}

		public IEnumerable<IRace> RaceList 
		{
			get
			{
				return _raceDictionary.Select(x => (IRace)x.Value).OrderBy(x => x.Date);
			}
		}
			
		public void AddRaceCode(string code)
		{
			if(!_raceDictionary.ContainsKey(code))
			{
				AutoUpdating = false;
				var race = new DropboxRace { Code = code };
				var manager = DBDatastoreManager.Manager(DBAccountManager.SharedManager.LinkedAccount);
				DBError error;

				var racestore = manager.CreateDatastore(out error);
				racestore.SetRole("public", DBRole.Editor);
				racestore.SyncAsync();
				race.DataStoreID = racestore.DatastoreId;
				_raceStore = racestore;
				_raceDictionary.Add (code, race);

				UpdateRaceInformation(race);
				SetRace(code);
				AutoUpdating = true;
			}
			LoadData();
		}

		public void SetRace(string code)
		{

			if(!_raceDictionary.ContainsKey(code))
				return;

			AutoUpdating = false;
			_race = _raceDictionary[code];
			DBError error;
			if(_raceStore != null)
			{
				_raceStore.Close();
			}
			var manager = DBDatastoreManager.Manager(DBAccountManager.SharedManager.LinkedAccount);
			_raceStore = manager.OpenDatastore(_race.DataStoreID, out error);
			_raceStore.Sync(out error);

			UpdateBoatInformation();
			UpdateEventData(true);

			_raceStore.AddObserver (_raceStore, () => {
				UpdateEventData(false);
			});
			AutoUpdating = true;
		}

		public IEnumerable<ISequenceItem> ItemList(string name, string token)
		{
			var items = _sequenceDictionary
				.Where(kvp => kvp.Key.Item1 == name && kvp.Key.Item2 == token)
				.Select(kvp => kvp.Value).ToList();
			return items;
		}

		public IEnumerable<IBoat> BoatList {
			get
			{
				return _boatDictionary.Values;
			}
		}

		// urgent - this will never be called if we're offline - we do need to be able to read from the offline version 
		public void LoadData ()
		{
			new NSObject().BeginInvokeOnMainThread(()=>{
				if(!AutoUpdating)
					return;
				AutoUpdating = false;
				var table = _generalStore.GetTable ("races");
				DBError error;
				var results = table.Query (null, out error);

				ProcessResults (results);

				AutoUpdating = true;
			});
		}

		public void UpdateEventData(bool justRead)
		{
			new NSObject().BeginInvokeOnMainThread(()=>{
				if(!AutoUpdating)
					return;
				AutoUpdating = false;
				var table = _raceStore.GetTable ("sequenceitems");
				DBError error;
				// todo - will probably want to filter out only the items from the race we're currently interested in 
				var results = table.Query (null, out error);

				ProcessSequenceItems (results, justRead);
				AutoUpdating = true;
			});
		}

		void ProcessSequenceItems(DBRecord[] results, bool justRead)
		{
			_sequenceRecords = results
				.ToDictionary(
					x => new Tuple<string,string,int>(x.Fields["Name"].ToString(), x.Fields["Token"].ToString(), ((NSNumber)x.Fields["StartNumber"]).IntValue),
					x => x);
			var writtenLocations = new List<Tuple<string,string>>();
			foreach(var kvp in _sequenceRecords)
			{
				ISequenceItem item; 
				_sequenceDictionary.TryGetValue(kvp.Key, out item);

				// this check means we are only going to be writing for a change we've not seen 
				if(item == null) 
				{
					IBoat boat;
					if(kvp.Key.Item3 < 0)
						boat = new BoatFactory().SetNumber(kvp.Key.Item3).Create();
					else
						boat = _boatDictionary[kvp.Key.Item3];
					item = kvp.Value.ToItem(boat);
					_sequenceDictionary.Add(kvp.Key, item);
					writtenLocations.Add(new Tuple<string, string>(kvp.Key.Item1, kvp.Key.Item2));
				}
				// todo - the super user will want to have all changes written 
			}

			if(!justRead)
				WriteDropboxFile(writtenLocations.Distinct());

			_raceStore.BeginInvokeOnMainThread (() => {
				if (ItemsListUpdated != null)
					ItemsListUpdated (this, EventArgs.Empty);
			});
		}

		void WriteDropboxFile(IEnumerable<Tuple<string,string>> locations)
		{
			// todo = unless you're a super user, only write the file for the location where you currently are 
			foreach(var tuple in locations)
			{
				var items = _sequenceDictionary
					.Where(kvp => kvp.Key.Item1 == tuple.Item1 && kvp.Key.Item2 == tuple.Item2)
					.Select(kvp => new JsonSequenceItem 
						{ 
							RaceCode = _race.Code, 
							LocationName = kvp.Key.Item1, 
							LocationToken = kvp.Key.Item2,
							StartNumber = kvp.Value.Boat.Number,
							TimeStamp = kvp.Value.TimeStamp,
							Notes = kvp.Value.Notes
					}).ToList();

				string json = JsonConvert.SerializeObject(items.OrderBy (cr => cr.TimeStamp));
				DBError error;
				string filename = string.Format("{0}-{1}-{2}.json", _race.Code, tuple.Item1, tuple.Item2);
				var path = DBPath.Root.ChildPath(_race.Code).ChildPath(filename);
				DBFileInfo fi = DBFilesystem.SharedFilesystem.FileInfoForPath(path, out error);
				DBFile file;
				if(fi == null || (error != null && error.Code == (int)DBErrorCode.NotFound))
					file = DBFilesystem.SharedFilesystem.CreateFile(path, out error);
				else
					file = DBFilesystem.SharedFilesystem.OpenFile(path, out error);

				if(file != null)
				{
					file.WriteString(json.ToString (), out error);
					file.Close();
				}
			}
		}

		void ProcessResults (DBRecord[] results)
		{
			_raceRecords = results.ToDictionary (x => x.Fields ["Code"].ToString (), x => x);
			foreach (var result in results) {
				var code = result.Fields ["Code"].ToString ();
				DropboxRace race;
				_raceDictionary.TryGetValue (code, out race);

				if (race == null) {
					race = result.ToRace ();
					_raceDictionary.Add (code, race);
				} 	else {
					race.Update (result);
				}

				UpdateRaceInformation(race);
			}
			_generalStore.BeginInvokeOnMainThread (() => {
				if (RaceListUpdated != null)
					RaceListUpdated (this, EventArgs.Empty);
			});
		}

		void UpdateBoatInformation()
		{
			DBError error;
			bool updated = false;
			var path = DBPath.Root;

			foreach(DBFileInfo i in DBFilesystem.SharedFilesystem.ListFolder(path, out error))
			{
				if(i.Path.Name.EndsWith(_race.Code + "-draw.json") && i.ModifiedTime > _race.BoatsUpdated)
				{
					Debug.WriteLine("need to update boats: " + i.Path.ToString());
					string json = DBFilesystem.SharedFilesystem.OpenFile(i.Path, out error).ReadString(out error);

					try
					{
						var boats = JsonConvert.DeserializeObject<List<JsonEntry>>(json);
						if(boats != null && boats.Count > 0)
						{
							_boatDictionary = boats
								.Select(b => 
									new BoatFactory()
									.SetNumber(b.StartNumber)
									.SetName(b.Name)
									.SetCategory(b.Category)
									.Create()
							)
								.ToDictionary(b => b.Number, b => b);
							_race.BoatsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}
				}
			}
			var table = _raceStore.GetTable("boats");

			if(updated)
			{
				DBRecord record;
				foreach(var kvp in _boatDictionary)
				{
					var bfields = kvp.Value.ToDictionary();
					if(_boatRecords.TryGetValue(kvp.Key, out record))
						record.Update(bfields);
					else
						table.GetOrInsertRecord(kvp.Key.ToString(), bfields, false, out error);
				}
				_raceStore.Sync(out error);
				_generalStore.Sync(out error);
			}

			var results = table.Query (null, out error);
			_boatRecords = results.ToDictionary(x => Int32.Parse(x.Fields["StartNumber"].ToString()), x => x);
			foreach(var result in results)
			{
				int start = Int32.Parse(result.Fields["StartNumber"].ToString());
				IBoat boat;
				_boatDictionary.TryGetValue(start, out boat);
				if(boat == null)
				{
					boat = result.ToBoat();
					_boatDictionary.Add(start, boat);
				} 
				// todo - need to make sure that updates to the boat list are reflected here. 

			}
			// race.BoatRecords = results.ToDictionary(x => new Tuple<int, string, string>(x.Fields["
		}

		void UpdateRaceInformation(DropboxRace race)
		{
			var path = DBPath.Root;
			DBError error;
			bool updated = false;

			foreach(DBFileInfo i in DBFilesystem.SharedFilesystem.ListFolder(path, out error))
			{
				// todo - will need to consider here the dangers of updating the boats during the middle of a race 

				// todo - KISS - we will only need the timing app to store boat number - location - token - timestamp 
				if(i.Path.Name.EndsWith(race.Code + "-details.json") && i.ModifiedTime > race.DetailsUpdated)
				{
					Debug.WriteLine("need to update details: " + i.Path.ToString());
					string json = DBFilesystem.SharedFilesystem.OpenFile(i.Path, out error).ReadString(out error);

					// todo - if this works, then abstract out the function and do it for the boats 
					try
					{
						var details = JsonConvert.DeserializeObject<List<JsonDetails>>(json).FirstOrDefault();
						if(details != null)
						{
							race.Name = details.Name;
							DateTime date;
							if(DateTime.TryParse(details.Date, out date))
								race.Date = date;
							race.Locations = 							 
								details
									.IntermediateLocations
									.Split(',')
									.Union(new List<string> { "start", "finish"})
									.Select(l => l.ToLowerInvariant())
									.Select(n => new LocationFactory().SetName(n).Create());
							race.DetailsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}

				}
			}				
			if(updated)
				Update(race);
		}

		// urgent - probably don't want this 
		public void DeleteAll ()
		{
			DBError error;

			// populated = false;
			var table = _generalStore.GetTable ("races");
			var results = table.Query (new NSDictionary (), out error);
			foreach (var result in results) {
				result.DeleteRecord ();
			}
			_generalStore.Sync (out error);
		}

		void Update (DropboxRace race)
		{
			DBRecord record;
			var hasRecord = _raceRecords.TryGetValue (race.Code, out record);
			var fields = race.ToDictionary ();
			var inserted = false;
			DBError error;
			if (hasRecord)
				record.Update (fields);
			else
				_generalStore.GetTable("races").GetOrInsertRecord (race.Code, fields, inserted, out error);
				
			_generalStore.Sync (out error);
		}

		public void Update()
		{
			_generalStore.SyncAsync ();
		}

		public void Reset()
		{
			DeleteAll();
		}

		public void LogATime(ILocation location, ISequenceItem item)
		{
			DBError error;
			var fields = item.ToDictionary(location);
			var table = _raceStore.GetTable("sequenceitems");
			string key = string.Format("{0}-{1}-{2}", location.Name, location.Token, item.Boat.Number);
			table.GetOrInsertRecord(key, fields, false, out error);
			_raceStore.Sync(out error);

			LastWriteSucceeded = error == null;
			if(LastWriteSucceeded)
				LastWriteTime = DateTime.Now;
		}

		public bool LastWriteSucceeded { get; private set ; }
		public DateTime LastWriteTime { get; private set ; }
		public string Name { get { return "Dropbox"; } }

		public IEnumerable<ILocation> LocationList {
			get
			{
				return _race == null ? new List<ILocation>() : _race.Locations; 
			}
		}
	}
}
