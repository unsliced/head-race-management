using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data.Internal.SQLite.Model 
{
	[Table("Races")] 
	class DbRace : BaseDatabaseObject<DbRace, Race>
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


		public Race As()
		{
			return new Race(Name, Code, Date);
		}
	}
}
