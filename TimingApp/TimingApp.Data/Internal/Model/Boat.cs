using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;

namespace TimingApp.Data.Internal.Model
{
	class Boat : BaseNotifyPropertyChanged, IBoat 
	{
		readonly string _name;
		readonly int _number;
		readonly string _category;
		readonly IRace _race;

		public Boat(int number, string name, string category)
		{
			_number = number;
			_name = name;
			_category = category;
			_race = race;
		}

		#region IEquatable implementation

		public bool Equals(IBoat other)
		{
			return Number == other.Number;
		}

		#endregion

		#region IBoat implementation

		public int Number { get { return _number; } }
		public string Name { get { return _name; } }
		public string Category { get { return _category; } }


		#endregion

		public void AddTime(ILocation location, ITimeStamp time)
		{
			_times.Add(location, time);
			OnPropertyChanged("Time"); 
		}
	}	
}
