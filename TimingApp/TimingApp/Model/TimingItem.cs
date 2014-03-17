using System;

namespace TimingApp.Model
{
	public class TimingItem 
	{
		readonly int _startNumber;
		readonly DateTime _time;

		public TimingItem(int startNumber, DateTime time, string notes)
		{
			_startNumber = startNumber;
			_time = time;
			Notes = notes;
		}

		public int StartNumber { get { return _startNumber; } } 
		public DateTime Time { get { return _time; } } 
		public string Notes { get; set; }
	}
}

