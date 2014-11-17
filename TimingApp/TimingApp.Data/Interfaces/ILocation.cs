using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface ILocation : IEquatable<ILocation> 
	{
		string Name { get; }
		string Token { get; } 
		IList<ISequenceItem> SequenceItems { get;} 
	}
}
