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
				new NSString("IntermediateLocations"),
			};
			NSDate d1 = DateTime.SpecifyKind(race.Date, DateTimeKind.Utc);
			NSDate d2 = DateTime.SpecifyKind(race.BoatsUpdated, DateTimeKind.Utc);
			NSDate d3 = DateTime.SpecifyKind(race.DetailsUpdated, DateTimeKind.Utc);
			var values = new NSObject[] {
				new NSString(race.Code),
				new NSString(race.Name), 
				d1, d2, d3, 
				new NSString(race.Racestore.DatastoreId), 
				new NSString(race.LocationNames.Aggregate((h,t) => string.Format("{0},{1}", h,t)))
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static NSDictionary ToDictionary (this IBoat boat)
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

		public static IBoat ToBoat (this DBRecord record)
		{
			return new BoatFactory()
				.SetName(record.Fields ["Name"].ToString())
				.SetNumber(((NSNumber)record.Fields["StartNumber"]).IntValue)
				.SetCategory(record.Fields ["Category"].ToString())
				.Create();

		}

		public static DropboxRace Update (this DropboxRace race, DBRecord record)
		{
			race.Code = record.Fields ["Code"].ToString ();
			if(record.Fields.ContainsKey(new NSString("IntermediateLocations")))
				race.SetLocationNames(record.Fields["FullName"].ToString().Split(','));
			if(record.Fields.ContainsKey(new NSString("FullName")))
				race.Name = record.Fields ["FullName"].ToString();
			if(record.Fields.ContainsKey(new NSString("DatastoreID")))
			{
				var id = record.Fields["DatastoreID"].ToString();
				if(race.Racestore == null || race.Racestore.DatastoreId != id)
				{
					DBError error;
					var manager = DBDatastoreManager.Manager(DBAccountManager.SharedManager.LinkedAccount);
					var racestore = manager.OpenDatastore(id, out error);
					racestore.Sync(out error);
					race.Racestore = racestore;
				}
			}
			if(record.Fields.ContainsKey(new NSString("RaceDate")))
			{
				var rd = record.Fields["RaceDate"];
				var nsd = (NSDate)rd;
				var dt = DateTime.SpecifyKind(nsd, DateTimeKind.Unspecified);
				race.Date = dt;
			}
			if(record.Fields.ContainsKey(new NSString("BoatsUpdated")))
				race.BoatsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["BoatsUpdated"]), DateTimeKind.Unspecified);
			if(record.Fields.ContainsKey(new NSString("DetailsUpdated")))
				race.DetailsUpdated = DateTime.SpecifyKind(((NSDate)record.Fields["DetailsUpdated"]), DateTimeKind.Unspecified);

			return race;
		}
	}
}
