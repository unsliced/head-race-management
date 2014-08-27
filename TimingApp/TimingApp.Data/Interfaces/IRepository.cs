using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimingApp.Data.Interfaces
{
	public interface IRepository 
	{
		void LogATime(ILocation location, IBoat boat, DateTime time, string notes);
		bool LastWriteSucceeded { get; } 
		DateTime LastWriteTime { get; } 
		string Name { get; } 
	}
}
