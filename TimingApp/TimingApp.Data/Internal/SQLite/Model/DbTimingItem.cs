using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data.Internal.SQLite.Model 
{
	[Table("TimingItems")]
	class DbTimingItem : BaseDatabaseObject<DbTimingItem, TimeStamp>
	{
		public DbTimingItem()
		{
		}

		string _race;
		[Column("_race")]
		public string Race { get { return _race; } set { SetField(ref _race, value, "Race"); } } 

		string _token;
		[Column("_token")]
		public string Token { get { return _token; } set { SetField(ref _token, value, "Token"); } } 

		string _location;
		[Column("_location")]
		public string Location { get { return _location; } set { SetField(ref _location, value, "Location"); } } 

		int _startNumber;
		[Column("_startNumber")]
		public int StartNumber { get { return _startNumber; } set { SetField(ref _startNumber, value, "StartNumber"); } } 

		DateTime _time;
		[Column("_time")]
		public DateTime Time { get { return _time; } set { SetField(ref _time, value, "Time"); } } 

		int _ms;
		[Column("_ms")]
		public int Milliseconds { get { return _ms; } set { SetField(ref _ms, value, "MS"); } } 

		bool _sequence;
		[Column("_sequence")]
		public bool Sequence { get { return _sequence; } set { SetField(ref _sequence, value, "Sequence"); } } 

		string _notes;
		[Column("_notes")]
		public string Notes { get { return _notes; } set { SetField(ref _notes, value, "Notes"); } } 

		public static DbTimingItem Create(IRace race, ILocation location, IBoat boat, DateTime now, string notes) 
		{
			return new DbTimingItem {
				Race = race.Code,
				Token = location.Code,
				Location = location.Endpoint.ToString(),
				Sequence = location.Sequence,
				StartNumber = boat.Number,
				Time = now,
				Milliseconds = now.Millisecond,
				Notes = notes
			};	
		}

		public TimeStamp As(IBoat boat, ILocation location)
		{
			return new TimeStamp(Time, Notes, boat, location);
		}
	}
}
