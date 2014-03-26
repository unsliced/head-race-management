using System;

namespace TimingApp.Model
{
	public class TimingItem 
	{
		public TimingItem(string race, string location, string gps, string token, int startNumber, DateTime time, string notes)
		{
			StartNumber = startNumber;
			Time = time;
			Race = race;
			Token = token;
			Location = location;
			GPS = gps;
			Notes = notes;
		}

		public string Race { get; set; }
		public string Token { get; set; }
		public string Location { get; set; }
		public string GPS { get; set; }
		public int StartNumber { get; set; } 
		public DateTime Time { get; set; } 
		public string Notes { get; set; }

		public string FileNameStub { get { return string.Format ("{0}.{1}.{2}", Race, Location, Token); } } 
	}
}

