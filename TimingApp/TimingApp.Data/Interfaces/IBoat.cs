using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface IBoat : IEquatable<IBoat>, INotifyPropertyChanged  
	{
		int Number { get; }
		string Name { get; }
		string Category { get; } 
		// idea: include an index code to display graphics/blade colour 

		string PrettyName { get; }

	}
}
