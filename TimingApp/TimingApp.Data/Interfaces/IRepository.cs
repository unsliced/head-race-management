using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimingApp.Data.Interfaces
{
	public interface IRepository 
	{
		void LogATime(ILocation location, ISequenceItem item);
		bool LastWriteSucceeded { get; } 
		DateTime LastWriteTime { get; } 
		string Name { get; } 
		void Update();
		event EventHandler RaceListUpdated;

		void AddRaceCode(string code);

		IEnumerable<IRace> RaceList { get; } 
		IEnumerable<ILocation> LocationList { get; } 

		// not sure we need to set a location, given that logatime takes it 
		// void SetLocation(ILocation location);

		void SetRace(string code);
		IEnumerable<IBoat> BoatList { get; }
		IEnumerable<ISequenceItem> ItemList(string name, string code);
	}
}
