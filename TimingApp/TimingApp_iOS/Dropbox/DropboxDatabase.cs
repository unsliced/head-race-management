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
		IDictionary<string,DropboxRace> _raceDictionary = new Dictionary<string, DropboxRace> ();
		IRace _race = null;

		static DropboxDatabase shared;

		// todo - what's this for? 
		public bool AutoUpdating { get; set; }

		public static DropboxDatabase Shared {
			get {
				if (shared == null)
					shared = new DropboxDatabase ();
				return shared;
			}
		}

		DBDatastore store;

		public DropboxDatabase ()
		{
			_raceDictionary = new Dictionary<string, DropboxRace>();
		}

		public void Init ()
		{
			if (store != null)
				return;
			DBError error;
			var manager = DBDatastoreManager.Manager(DBAccountManager.SharedManager.LinkedAccount);
			if(string.IsNullOrEmpty(DatastoreId))
			{
				store = manager.CreateDatastore(out error);
				Debug.WriteLine("Datastore created, take a note of this:", store.DatastoreId);
				store.SetRole("public", DBRole.Editor);
			}
			else
				store = manager.OpenDatastore(DatastoreId, out error);

			store.Sync (out error);

			// DeleteAll();
			store.Sync (out error);

			store.AddObserver (store, () => {
				LoadData ();
			});
			AutoUpdating = true;
		}

		public event EventHandler ListUpdated;

		public IEnumerable<IRace> IFactory<IEnumerable<IRace>>.Create()
		{
			return _raceDictionary.Select (x => (IRace)x.Value).OrderBy (x => x.Date);
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
				race.Racestore = racestore;
				race.Racestore.SetRole("public", DBRole.Editor);
				race.Racestore.SyncAsync();

				_raceDictionary.Add (code, race);

				UpdateRaceInformation(race);
				AutoUpdating = true;
			}
			LoadData();
		}

		public void SetRace(IRace race)
		{
			_race = race;
		}

		// urgent - this will never be called if we're offline - we do need to be able to read from the offline version 
		public void LoadData ()
		{
			new NSObject().BeginInvokeOnMainThread(()=>{
				if(!AutoUpdating)
					return;
				AutoUpdating = false;
				var table = store.GetTable ("races");
				DBError error;
				var results = table.Query (null, out error);

				ProcessResults (results);
				AutoUpdating = true;
			});
		}

		void ProcessResults (DBRecord[] results)
		{
			_raceRecords = results.ToDictionary (x => x.Fields ["Code"].ToString (), x => x);
			foreach (var result in results) {
				var code = result.Fields ["Code"].ToString ();
				DropboxRace race;
				_raceDictionary.TryGetValue (code, out race);

				if (race == null) {
					// should never come in to here 
					race = result.ToRace ();
					_raceDictionary.Add (code, race);
				} 	else {
					race.Update (result);
				}

				UpdateRaceInformation(race);
				UpdateBoatInformation(race);
			}
			store.BeginInvokeOnMainThread (() => {
				if (ListUpdated != null)
					ListUpdated (this, EventArgs.Empty);
			});
		}

		void UpdateBoatInformation(DropboxRace race)
		{
			DBError error;
			var table = race.Racestore.GetTable("boats");
			var results = table.Query (null, out error);
			race.BoatRecords = results.ToDictionary(x => Int32.Parse(x.Fields["StartNumber"].ToString()), x => x);
			foreach(var result in results)
			{
				int start = Int32.Parse(result.Fields["StartNumber"].ToString());
				IBoat boat;
				race.BoatDictionary.TryGetValue(start, out boat);
				if(race == null)
				{
					boat = result.ToBoat();
					race.BoatDictionary.Add(start, boat);
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
							race.SetLocationNames(
								details
									.IntermediateLocations
									.Split(',')
									.Union(new List<string> { "start", "finish"})
									.Select(l => l.ToLowerInvariant())
							);
							race.DetailsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}

				}
				if(i.Path.Name.EndsWith(race.Code + "-draw.json") && i.ModifiedTime > race.BoatsUpdated)
				{
					Debug.WriteLine("need to update boats: " + i.Path.ToString());
					string json = DBFilesystem.SharedFilesystem.OpenFile(i.Path, out error).ReadString(out error);

					try
					{
						var boats = JsonConvert.DeserializeObject<List<JsonEntry>>(json);
						if(boats != null && boats.Count > 0)
						{
							race.BoatDictionary = boats
								.Select(b => 
									new BoatFactory()
										.SetNumber(b.StartNumber)
										.SetName(b.Name)
										.SetCategory(b.Category)
										.Create()
									)
								.ToDictionary(b => b.Number, b => b);
							race.BoatsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}
				}
			}				
			if(updated)
				Update(race, true);
		}

		// urgent - probably don't want this 
		public void DeleteAll ()
		{
			DBError error;

			// populated = false;
			var table = store.GetTable ("races");
			var results = table.Query (new NSDictionary (), out error);
			foreach (var result in results) {
				result.DeleteRecord ();
			}
			store.Sync (out error);
		}

		void Update (DropboxRace race, bool boats)
		{
			DBRecord record;
			var hasRecord = _raceRecords.TryGetValue (race.Code, out record);
			var fields = race.ToDictionary ();
			var inserted = false;
			DBError error;
			if (hasRecord)
				record.Update (fields);
			else
				store.GetTable("races").GetOrInsertRecord (race.Code, fields, inserted, out error);
				
			store.Sync (out error);

			if(!boats)
				return;

			var table = race.Racestore.GetTable("boats");
			foreach(var kvp in race.BoatDictionary)
			{
				var bfields = kvp.Value.ToDictionary();
				if(race.BoatRecords.TryGetValue(kvp.Key, out record))
					record.Update(fields);
				else
					table.GetOrInsertRecord(kvp.Key.ToString(), bfields, false, out error);
			}
			race.Racestore.Sync(out error);
		}

		public void Update()
		{
			store.SyncAsync ();
		}

		public void Reset()
		{
			DeleteAll();
		}
	}
}
