using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Data.Internal.Model;
using TimingApp.Data.Interfaces;

namespace TimingApp.Data.Internal.SQLite.Model 
{
	[Table("Boats")]
	class DbBoat : BaseDatabaseObject<DbBoat, Boat>
	{
		string _race;
		[Column("_race")]
		public string RaceCode { get { return _race; } set { SetField(ref _race, value, "Race"); } } 

		int _number;
		[Column("_number")]
		public int Number { get { return _number; } set { SetField(ref _number, value, "Number"); } } 

		string _name;
		[Column("_name")]
		public string Name { get { return _name; } set { SetField(ref _name, value, "Name"); } } 

		string _category;
		[Column("_category")]
		public string Category { get { return _category; } set { SetField(ref _category, value, "Category"); } } 	

		public Boat As(IRace race, ILocation favour)
		{
			return new Boat(Number, Name, Category); // , race, favour);
		}
	}	
}
