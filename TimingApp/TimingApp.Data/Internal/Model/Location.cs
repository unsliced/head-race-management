using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;

namespace TimingApp.Data.Internal.Model
{

	class Location : ILocation 
	{
		readonly string _name;
		readonly string _token;
		readonly IList<ISequenceItem> _sequenceItems; 

		public Location(string name, string token)
		{
			_name = name;
			_token = token;
			_sequenceItems = new List<ISequenceItem>();
		}

		#region IEquatable implementation
		public bool Equals(ILocation other)
		{
			return Name == other.Name && Token == other.Token;
		}
		#endregion

		#region ILocation implementation

		public string Name { get { return _name; } }
		public string Token { get { return _token; } }
		public IList<ISequenceItem> SequenceItems { get { return _sequenceItems; } } 

		#endregion
	}
	
}
