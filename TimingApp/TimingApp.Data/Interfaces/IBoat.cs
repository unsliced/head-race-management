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
		IEnumerable<ITimeStamp> Time { get; } 
		IRace Race { get; } 
	}

}
