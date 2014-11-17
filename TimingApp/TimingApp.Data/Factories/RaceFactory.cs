using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;
using System.Collections.Generic;

namespace TimingApp.Data.Factories
{
		
	public class RaceFactory : IFactory<IRace>
	{
		string _name;
		string _code;
		DateTime _date;
		IEnumerable<IBoat> _boats;
		IEnumerable<ILocation> _locations;

		public RaceFactory SetName(string name)
		{
			_name = name;
			return this;
		}

		public RaceFactory SetCode(string code)
		{
			_code = code;
			return this;
		}

		public RaceFactory SetDate(DateTime date)
		{
			_date = date;
			return this;
		}

		public RaceFactory SetBoats(IEnumerable<IBoat> boats)
		{
			_boats = boats;
			return this;
		}

		public RaceFactory SetLocations(IEnumerable<ILocation> locations)
		{
			_locations = locations;
			return this;
		}

		public IRace Create() {
			return new Race(_name, _code, _date, _boats, _locations);
		}
	}
}
