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
	}
}
