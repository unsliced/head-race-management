using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TimingApp.Data.Internal.Model
{
	// todo - consider removing this
	class Race : IRace 
	{
		readonly DateTime _date;
		readonly string _name;
		readonly string _code;
		readonly IList<IBoat> _boats;
		readonly IList<ILocation> _locations;

		public Race(string name, string code, DateTime date, 
			IEnumerable<IBoat> boats, 
			IEnumerable<ILocation> locations)
		{
			_date = date;
			_name = name;
			_code = code;
			_boats = boats.ToList();
			_locations = locations.ToList();
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
