using System;
using System.Collections.Generic;
using TimingApp.Data.Enums;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface ILocation : IEquatable<ILocation> 
	{
		Endpoint Endpoint { get; } 
		string Code { get; }
		bool Sequence { get; } 
		IRace Race { get; } 
		IList<ITimeStamp> Unidentified { get;} 
	}
}
