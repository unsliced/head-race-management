using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;
using TimingApp.Data.Enums;

namespace TimingApp.Data.Internal.Model
{

	class Location : ILocation 
	{
		readonly string _code;
		readonly Endpoint _endpoint;
		readonly bool _sequence;
		readonly IRace _race;

		public Location(string code, Endpoint endpoint, bool sequence, IRace race)
		{
			_code = code;
			_endpoint = endpoint;
			_sequence = sequence;
			_race = race;
		}

		#region IEquatable implementation
		public bool Equals(ILocation other)
		{
			return Endpoint == other.Endpoint && Race.Code == other.Race.Code && Code == other.Code && Sequence == other.Sequence;
		}
		#endregion

		#region ILocation implementation

		public Endpoint Endpoint {get { return _endpoint; } }
		public string Code { get { return _code; } }
		public bool Sequence { get { return _sequence; } }
		public IRace Race { get { return _race; } }

		#endregion
	}
	
}
