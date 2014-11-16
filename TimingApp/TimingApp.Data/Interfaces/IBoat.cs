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
		DateTime TimeStamp { get; } 
		string Notes { get; } 

		void AddATime(DateTime timestamp, string notes);
		// idea: include an index code to display graphics/blade colour 
	}
}
