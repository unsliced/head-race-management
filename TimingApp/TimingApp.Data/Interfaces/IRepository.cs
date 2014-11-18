using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimingApp.Data.Interfaces
{
	public interface IRepository 
		: IFactory<IEnumerable<IRace>>, IFactory<IEnumerable<ILocation>>
	{
		void LogATime(ILocation location, ISequenceItem item);
		bool LastWriteSucceeded { get; } 
		DateTime LastWriteTime { get; } 
		string Name { get; } 
		void Update();
		event EventHandler RaceListUpdated;

		void AddRaceCode(string code);

		void SetRace(IRace race);
		// not sure we need to set a location, given that logatime takes it 
		// void SetLocation(ILocation location);
	}
}
