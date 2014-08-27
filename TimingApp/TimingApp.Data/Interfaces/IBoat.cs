using System;
using System.Collections.Generic;
using TimingApp.Data.Enums;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface IBoat : IEquatable<IBoat>, INotifyPropertyChanged  
	{
		int Number { get; }
		string Name { get; }
		string Category { get; } 
		IDictionary<ILocation, ITimeStamp> Times { get; } 
		IRace Race { get; } 
		string VisibleTime { get; }
		// idea: include an index code to display graphics/blade colour 
	}
}
