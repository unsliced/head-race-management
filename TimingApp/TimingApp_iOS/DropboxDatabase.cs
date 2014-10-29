using System;
using DropBoxSync.iOS;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using TimingApp.Data.Interfaces;
using System.Diagnostics;

namespace TimingApp_iOS
{
	public class DropboxRace : IRace
	{
		public string Code { get; set; } 
		public string Name { get; set; } 
		public DateTime Date { get; set; } 
		public DateTime BoatsUpdated { get; set; } 
		public DateTime DetailsUpdated { get; set; } 

		public IEnumerable<IBoat> Boats { get { throw new NotImplementedException(); } }

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
		IDictionary<string,DBRecord> _records = new Dictionary<string, DBRecord> ();
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
			store = DBDatastore.OpenDefaultStore (DBAccountManager.SharedManager.LinkedAccount, out error);
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

		// todo - need the ability to add in a new race code

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
			_records = results.ToDictionary (x => x.Fields ["Code"].ToString (), x => x);
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
//			var manager = DBDatastoreManager.LocalManager(DBAccountManager.SharedManager);
//			DBError error;
//			var racestore = manager.OpenOrCreateDatastore(code, out error);
//			racestore.Sync(error);
//			var table = racestore.GetTable("locations");
			var path = DBPath.Root;
			DBError error;
			foreach(DBFileInfo i in DBFilesystem.SharedFilesystem.ListFolder(path, out error))
			{
				// todo - will need to consider here the dangers of updating the boats during the middle of a race 

				// todo - KISS - we will only need the timing app to store boat number - location - token - timestamp 
				bool updated = false;
				if(i.Path.Name == race + "-details.json" && i.ModifiedTime > race.DetailsUpdated)
				{
					// todo - open the json file to consume the details object 
					Debug.WriteLine("need to update details: " + i.Path.ToString());

					updated = true;
				}
				if(i.Path.Name == race + "-draw.json" && i.ModifiedTime > race.DetailsUpdated)
				{
					// todo - open the json file to repopulate the boats information 
					Debug.WriteLine("need to update boats: " + i.Path.ToString());
					updated = true;
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
			var hasRecord = _records.TryGetValue (race.Code, out record);
			var fields = race.ToDictionary ();
			var inserted = false;
			DBError error;
			if (hasRecord)
				record.Update (fields);
			else
				store.GetTable("races").GetOrInsertRecord (race.Code, fields, inserted, out error);


			store.SyncAsync ();
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
			};
			NSDate d1 = DateTime.SpecifyKind(race.Date, DateTimeKind.Utc);
			NSDate d2 = DateTime.SpecifyKind(race.BoatsUpdated, DateTimeKind.Utc);
			NSDate d3 = DateTime.SpecifyKind(race.DetailsUpdated, DateTimeKind.Utc);
			var values = new NSObject[] {
				new NSString(race.Code),
				new NSString(race.Name), 
				d1, d2, d3
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static DropboxRace ToRace (this DBRecord record)
		{
			return new DropboxRace ().Update (record);

		}

		public static DropboxRace Update (this DropboxRace race, DBRecord record)
		{
			race.Code = record.Fields ["Code"].ToString ();
			race.Name = record.Fields ["FullName"].ToString();
			race.Date = DateTime.SpecifyKind(((NSDate)record.Fields["RaceDate"]), DateTimeKind.Unspecified);
			if(record.Fields.ContainsKey(new NSString("BoatsUpdated")))
				race.BoatsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["BoatsUpdated"]), DateTimeKind.Unspecified);
				if(record.Fields.ContainsKey(new NSString("DetailsUpdated")))
				race.BoatsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["DetailsUpdated"]), DateTimeKind.Unspecified);

			return race;
		}
	}
}

