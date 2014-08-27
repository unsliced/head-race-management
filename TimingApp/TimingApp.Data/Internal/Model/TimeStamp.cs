using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;
using TimingApp.Data.Enums;

namespace TimingApp.Data.Internal.Model
{
	class TimeStamp : ITimeStamp
	{
		readonly DateTime _timeStamp;
		readonly string _notes;
		readonly IBoat _boat;
		readonly ILocation _location;

		public TimeStamp(DateTime timestamp, string notes, IBoat boat, ILocation location) 
		{
			_timeStamp = timestamp;
			_notes = notes;
			_boat = boat;
			_location = location;
		}

		#region ITime implementation

		public DateTime Time { get { return _timeStamp; } }
		public string Notes { get { return _notes; } }
		public IBoat Boat { get { return _boat; } }		
		public ILocation Location { get { return _location; } }
		
		#endregion
	}
}

