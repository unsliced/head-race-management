using System;
using System.Collections.Generic;
using TimingApp.Data.Enums;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface ITimeStamp 
 	{
		DateTime Time { get; } 
		string Notes { get; }
		IBoat Boat { get; } 
		ILocation Location { get; } 
		// todo: add a GPS marker (in addition to the location) 
	}
}
