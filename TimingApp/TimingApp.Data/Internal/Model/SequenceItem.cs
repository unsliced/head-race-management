using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;

namespace TimingApp.Data.Internal.Model
{
	class SequenceItem : ISequenceItem
	{
		readonly DateTime _timeStamp;
		readonly string _notes;
		readonly IBoat _boat;

		public SequenceItem(IBoat boat, DateTime timestamp, string notes) 
		{
			_timeStamp = timestamp;
			_notes = notes;
			_boat = boat;
		}

		#region ITime implementation

		public DateTime TimeStamp { get { return _timeStamp; } }
		public string Notes { get { return _notes; } }
		public IBoat Boat { get { return _boat; } }		

		#endregion
	}
}

