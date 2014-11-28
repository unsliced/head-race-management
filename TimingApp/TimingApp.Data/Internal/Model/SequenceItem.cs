using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;

namespace TimingApp.Data.Internal.Model
{
	class SequenceItem : ISequenceItem
	{
		readonly DateTime _timeStamp;
		readonly IBoat _boat;

		public SequenceItem(IBoat boat, DateTime timestamp, string notes) 
		{
			_timeStamp = timestamp;
			Notes = notes;
			_boat = boat;
		}

		#region ITime implementation

		public DateTime TimeStamp { get { return _timeStamp; } }
		public string Notes { get; set;  }
		public IBoat Boat { get { return _boat; } }		
	
		public string PrettyTime {
			get
			{
				return TimeStamp.ToString("HH:mm:ss.fff");
			}
		}
		#endregion
	}
}

