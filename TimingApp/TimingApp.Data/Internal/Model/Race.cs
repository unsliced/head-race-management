using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;

namespace TimingApp.Data.Internal.Model
{
	class Race : IRace 
	{
		readonly DateTime _date;
		readonly string _name;
		readonly string _code;
		readonly IList<IBoat> _boats;
		readonly IList<ILocation> _locations;

		public Race(string name, string code, DateTime date)
		{
			_date = date;
			_name = name;
			_code = code;
			_boats = new List<IBoat>();
			_locations = new List<ILocation>();
		}

		public void AddBoat(IBoat boat)
		{
			_boats.Add(boat);
		}

		public void AddLocation(ILocation location)
		{
			if(_locations.Contains(location))
				return;
			_locations.Add(location);
		}

		#region IRace implementation

		public DateTime Date { get { return _date; } }
		public string Name { get { return _name; } }
		public string Code { get { return _code; } }

		public IEnumerable<IBoat> Boats { get { return _boats; } }
		public IEnumerable<ILocation> Locations { get { return _locations; } }

		#endregion
	}
	
}
