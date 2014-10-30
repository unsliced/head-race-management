using System;
using DropBoxSync.iOS;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace TimingApp_iOS
{
	public class DropboxBoat : IBoat
	{
		#region INotifyPropertyChanged implementation

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region IEquatable implementation

		public bool Equals(IBoat other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBoat implementation

		public int Number { get; set; } 
		public string Name { get; set; } 
		public string Category { get; set; } 
		public IDictionary<ILocation, ITimeStamp> Times { 
			get
			{
				throw new NotImplementedException();
			}
		}

		public IRace Race {
			get
			{
				throw new NotImplementedException();
			}
		}

		public string VisibleTime {
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion


	}

	public class DropboxRace : IRace
	{
		public string Code { get; set; } 
		public string Name { get; set; } 
		public DateTime Date { get; set; } 
		public DateTime BoatsUpdated { get; set; } 
		public DateTime DetailsUpdated { get; set; } 
		public string DatastoreId { get; set; } 

		// hack - hide these 
		public IDictionary<int,DBRecord> Records = new Dictionary<int, DBRecord> ();
		public IDictionary<int,DropboxBoat> BoatDictionary = new Dictionary<int, DropboxBoat> ();

		public IEnumerable<IBoat> Boats { get { return BoatDictionary.Values.ToList(); } }

		public IEnumerable<ILocation> Locations { get { throw new NotImplementedException(); } }

		public DropboxRace() 
		{
		}

		public DropboxRace(IRace race)
		{
			Code = race.Code;
			Name = race.Name;
			Date = race.Date;
			BoatsUpdated = DateTime.MinValue;
			DetailsUpdated = DateTime.MinValue;
		}
	}

	public class DropboxDatabase : IFactory<IRace>
	{
		const string DatastoreId = ".8K0ATSTNNGqlZvqwFi7HQm_Fd6oA9tbLT3ZPPWbGKHg";

		IDictionary<string,DBRecord> _raceRecords = new Dictionary<string, DBRecord> ();
		IDictionary<string,DropboxRace> _raceDictionary = new Dictionary<string, DropboxRace> ();

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
			store.AddObserver (store, () => {
				LoadData ();
			});
			AutoUpdating = true;
		}

		public event EventHandler ListUpdated;

		public IEnumerable<IRace> Create()
		{
			return _raceDictionary.Select (x => (IRace)x.Value).OrderBy (x => x.Date);
		}

		public void Add(string code)
		{
			if(!_raceDictionary.ContainsKey(code))
			{
				var race = new DropboxRace { Code = code };
				var fields = race.ToDictionary ();
				var inserted = false;
				DBError error;
				store.GetTable("races").GetOrInsertRecord (race.Code, fields, inserted, out error);

				store.SyncAsync ();

				var manager = DBDatastoreManager.LocalManager(DBAccountManager.SharedManager);

				var racestore = manager.CreateDatastore(out error);
				race.DatastoreId = racestore.DatastoreId;
			}
			LoadData();
		}

		// urgent - this will never be called if we're offline - we do need to be able to read from the offline version 
		public void LoadData ()
		{
			new NSObject().BeginInvokeOnMainThread(()=>{
				var table = store.GetTable ("races");
				DBError error;
				var results = table.Query (null, out error);

				ProcessResults (results);
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

			}
			store.BeginInvokeOnMainThread (() => {
				if (ListUpdated != null)
					ListUpdated (this, EventArgs.Empty);
			});
		}

		void UpdateRaceInformation(DropboxRace race)
		{
			var path = DBPath.Root;
			DBError error;
			foreach(DBFileInfo i in DBFilesystem.SharedFilesystem.ListFolder(path, out error))
			{
				// todo - will need to consider here the dangers of updating the boats during the middle of a race 

				// todo - KISS - we will only need the timing app to store boat number - location - token - timestamp 
				bool updated = false;
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
							// todo - deal with the list of locations 
							race.DetailsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}

				}
				if(i.Path.Name.EndsWith(race.Code + "-draw.json") && i.ModifiedTime > race.DetailsUpdated)
				{
					Debug.WriteLine("need to update boats: " + i.Path.ToString());
					string json = DBFilesystem.SharedFilesystem.OpenFile(i.Path, out error).ReadString(out error);

					try
					{
						var boats = JsonConvert.DeserializeObject<List<JsonBoat>>(json);
						if(boats != null && boats.Count > 0)
						{
							race.BoatDictionary = boats
								.Select(b => 
									new DropboxBoat { Number = b.StartNumber, Category = b.Category, Name = b.Name})
								.ToDictionary(b => b.Number, b => b);
							race.BoatsUpdated = DateTime.Now;
							updated = true;
						}

					} catch(Exception ex)
					{
						updated = false;
					}
				}
				if(updated)
					Update(race);
			}				
		}

		// urgent - probably don't want this 
		public void DeleteAll ()
		{
			// populated = false;
			var table = store.GetTable ("races");
			DBError error;
			var results = table.Query (new NSDictionary (), out error);
			foreach (var result in results) {
				result.DeleteRecord ();
			}
			store.Sync (out error);
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
				store.GetTable("races").GetOrInsertRecord (race.Code, fields, inserted, out error);
				
			store.SyncAsync ();

			var manager = DBDatastoreManager.LocalManager(DBAccountManager.SharedManager);

			var racestore = manager.OpenDatastore(race.DatastoreId, out error);
			racestore.Sync(out error);
			var table = racestore.GetTable("boats");
			foreach(var kvp in race.BoatDictionary)
			{
				var bfields = kvp.Value.ToDictionary();
				if(race.Records.TryGetValue(kvp.Key, out record))
					record.Update(fields);
				else
					table.GetOrInsertRecord(kvp.Key.ToString(), bfields, false, out error);
			}
			racestore.SyncAsync();
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

	public static class DropboxHelper
	{
		public static NSDictionary ToDictionary (this DropboxRace race)
		{
			var keys = new NSString[] {
				new NSString("Code"),
				new NSString("FullName"),
				new NSString("RaceDate"),
				new NSString("BoatsUpdated"),
				new NSString("DetailsUpdated"),
				new NSString("DatastoreID"),
			};
			NSDate d1 = DateTime.SpecifyKind(race.Date, DateTimeKind.Utc);
			NSDate d2 = DateTime.SpecifyKind(race.BoatsUpdated, DateTimeKind.Utc);
			NSDate d3 = DateTime.SpecifyKind(race.DetailsUpdated, DateTimeKind.Utc);
			var values = new NSObject[] {
				new NSString(race.Code),
				new NSString(race.Name), 
				d1, d2, d3, 
				new NSString(race.DatastoreId)
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static NSDictionary ToDictionary (this DropboxBoat boat)
		{
			var keys = new NSString[] {
				new NSString("StartNumber"),
				new NSString("Name"),
				new NSString("Category"),
			};
			var values = new NSObject[] {
				new NSNumber(boat.Number),
				new NSString(boat.Name), 
				new NSString(boat.Category), 
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static DropboxRace ToRace (this DBRecord record)
		{
			return new DropboxRace ().Update (record);

		}

		public static DropboxBoat ToBoat (this DBRecord record)
		{
			return new DropboxBoat ().Update (record);

		}

		public static DropboxRace Update (this DropboxRace race, DBRecord record)
		{
			race.Code = record.Fields ["Code"].ToString ();
			if(record.Fields.ContainsKey(new NSString("FullName")))
				race.Name = record.Fields ["FullName"].ToString();
			if(record.Fields.ContainsKey(new NSString("DatastoreID")))
				race.Name = record.Fields ["DatastoreID"].ToString();
			if(record.Fields.ContainsKey(new NSString("RaceDate")))
				race.Date = DateTime.SpecifyKind(((NSDate)record.Fields["RaceDate"]), DateTimeKind.Unspecified);
			if(record.Fields.ContainsKey(new NSString("BoatsUpdated")))
				race.BoatsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["BoatsUpdated"]), DateTimeKind.Unspecified);
			if(record.Fields.ContainsKey(new NSString("DetailsUpdated")))
				race.BoatsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["DetailsUpdated"]), DateTimeKind.Unspecified);

			return race;
		}

		public static DropboxBoat Update (this DropboxBoat boat, DBRecord record)
		{
			boat.Number = ((NSNumber)record.Fields["StartNumber"]).IntValue;
			boat.Name = record.Fields ["Name"].ToString();
			boat.Category = record.Fields ["Category"].ToString();

			return boat;
		}


	}
}

