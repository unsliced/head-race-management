using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface ISequenceItem 
 	{
		string Notes { get; set; }
		DateTime TimeStamp { get; } 
		IBoat Boat { get; } 
		// todo: add a GPS marker (in addition to the location) 
		string PrettyTime { get; } 
	}
}
