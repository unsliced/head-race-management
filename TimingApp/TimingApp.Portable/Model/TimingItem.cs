using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Portable.Database;

namespace TimingApp.Portable.Model
{
	// TODO: this should probably be internal, but let's not get too hung up on it at this point 
	[Table("TimingItems")]
	public class TimingItem : BaseDatabaseObject<TimingItem>
	{
		public TimingItem()
		{
		}

		[Obsolete] 
		public TimingItem(string race, string location, string gps, string token, int startNumber, DateTime time, string notes)
		{
			StartNumber = startNumber;
			Time = time;
			Race = race;
			Token = token;
			Location = location;
			GPS = gps;
			Notes = notes;
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

		string _gps;
		[Column("_gps")]
		public string GPS { get { return _gps; } set { SetField(ref _gps, value, "GPS"); } } 

		int _startNumber;
		[Column("_startNumber")]
		public int StartNumber { get { return _startNumber; } set { SetField(ref _startNumber, value, "StartNumber"); } } 

		DateTime _time;
		[Column("_time")]
		public DateTime Time { get { return _time; } set { SetField(ref _time, value, "Time"); } } 

		string _notes;
		[Column("_notes")]
		public string Notes { get { return _notes; } set { SetField(ref _notes, value, "Notes"); } } 

		[Ignore]
		public string FileNameStub { get { return string.Format ("{0}.{1}.{2}", Race, Location, Token); } } 

		// todo: probably want to use the Boat as a foreign key based on Race/StartNumber 
		[Ignore]
		public Boat Boat { get; set; } 
	}

	// fixme: this should probably be iequatable 
	[Table("Boats")]
	public class Boat : BaseDatabaseObject<Boat>
	{
		string _race;
		[Column("_race")]
		public string Race { get { return _race; } set { SetField(ref _race, value, "Race"); } } 

		int _number;
		[Column("_number")]
		public int Number { get { return _number; } set { SetField(ref _number, value, "Number"); } } 

		string _name;
		[Column("_name")]
		public string Name { get { return _name; } set { SetField(ref _name, value, "Name"); } } 

		string _category;
		[Column("_category")]
		public string Category { get { return _category; } set { SetField(ref _category, value, "Category"); } } 
	}

	[Table("Races")] 
	public class Race : BaseDatabaseObject<Race>
	{
		string _code;
		[Column("_code")]
		public string Code { get { return _code; } set { SetField(ref _code, value, "Code"); } } 

		DateTime _date;
		[Column("_date")]
		public DateTime Date { get { return _date; } set { SetField(ref _date, value, "Date"); } } 

		string _name;
		[Column("_name")]
		public string Name { get { return _name; } set { SetField(ref _name, value, "Name"); } } 
	}
}

