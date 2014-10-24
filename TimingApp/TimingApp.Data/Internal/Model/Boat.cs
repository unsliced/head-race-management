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
		readonly IDictionary<ILocation, ITimeStamp> _times;
		readonly ILocation _favour;

		public Boat(int number, string name, string category, IRace race, ILocation favour)
		{
			_number = number;
			_name = name;
			_category = category;
			_race = race;
			_times = new Dictionary<ILocation, ITimeStamp>();
			_favour = favour;
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
		public IDictionary<ILocation, ITimeStamp> Times { get { return _times; } }
		public IRace Race { get { return _race; } }
		public string VisibleTime {
			get
			{ 
				return _times.ContainsKey(_favour) 
					? _times[_favour].Time.ToString("hh:mm:ss.fff")
					: "n/a" ;
			}
		}


		#endregion

		public void AddTime(ILocation location, ITimeStamp time)
		{
			_times.Add(location, time);
			OnPropertyChanged("Time"); 
		}
	}	
}