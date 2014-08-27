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
		readonly IList<ITimeStamp> _times;

		public Boat(int number, string name, string category, IRace race)
		{
			_number = number;
			_name = name;
			_category = category;
			_race = race;
			_times = new List<ITimeStamp>();
		}

		#region IEquatable implementation
		public bool Equals(IBoat other)
		{
			return Race.Code == other.Race.Code && Number == other.Number;
		}
		#endregion

		#region IBoat implementation

		public int Number { get { return _number; } }
		public string Name { get { return _name; } }
		public string Category { get { return _category; } }
		public IEnumerable<ITimeStamp> Time { get { return _times; } }
		public IRace Race { get { return _race; } }

		#endregion

		public void AddTime(ITimeStamp time)
		{
			_times.Add(time);
			OnPropertyChanged("Time"); 
		}
	}	
}
