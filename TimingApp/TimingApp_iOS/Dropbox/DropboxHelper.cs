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
				new NSString(race.DataStoreID), 
				new NSString(race
					.Locations
					.Select(l => l.Name)
					.Aggregate((h,t) => string.Format("{0},{1}", h,t)))
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static NSDictionary ToDictionary(this ISequenceItem item, ILocation location)
		{
			NSDate d1 = DateTime.SpecifyKind(item.TimeStamp, DateTimeKind.Utc);
			var keys = new NSString[] {
				new NSString("Name"),
				new NSString("Token"),
				new NSString("StartNumber"),
				new NSString("Timestamp"),
				new NSString("Milliseconds"),
				new NSString("Notes"),
			};
			var values = new NSObject[] {
				new NSString(location.Name),
				new NSString(location.Token), 
				new NSNumber(item.Boat.Number), 
				d1, 
				new NSNumber(item.TimeStamp.Millisecond), 
				new NSString(item.Notes), 
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

		public static ISequenceItem ToItem(this DBRecord record, IBoat boat)
		{
			var rd = record.Fields["Timestamp"];
			var nsd = (NSDate)rd;
			var dt = DateTime.SpecifyKind(nsd, DateTimeKind.Unspecified);
			if(record.Fields.ContainsKey(new NSString("Milliseconds")))
			{
				var ms = ((NSNumber)record.Fields["Milliseconds"]).IntValue;
				dt = dt.AddMilliseconds(ms);
			}
			return 
				new SequenceItemFactory()
					.SetBoat(boat)
					.SetTime(dt)
					.SetNotes(record.Fields["Notes"].ToString())
					.Create();
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
			{
				var locations = race.Locations.ToList();
				IList<ILocation> toAdd = new List<ILocation>();
				foreach(var location in 
					record
						.Fields["IntermediateLocations"]
						.ToString()
						.Split(',')
						.Union(new List<string>{"start","finish"}))
				{
					if(!locations.Select(l => l.Name).Contains(location))
						toAdd.Add(new LocationFactory().SetName(location).Create());
				}
				if(toAdd.Count > 0)
					race.Locations = locations.Union(toAdd);
			}
			if(record.Fields.ContainsKey(new NSString("DatastoreID")))
				race.DataStoreID = record.Fields ["DatastoreID"].ToString();
			if(record.Fields.ContainsKey(new NSString("FullName")))
				race.Name = record.Fields ["FullName"].ToString();
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
