using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimingApp.Data.Interfaces
{
	public interface IRace
	{
		DateTime Date { get;}
		string Name { get; }
		string Code { get; } 
		IEnumerable<IBoat> Boats { get; }
		IEnumerable<ILocation> Locations { get; }

		IEnumerable<string> LocationNames { get; } 
	}
}
