using System;
using TimingApp.Portable.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimingApp.Portable.DataLayer
{
	public interface IRepository 
	{
		IEnumerable<Race> AllRaces { get; }
		IEnumerable<TimingItem> TimingItems(Race race);
		IEnumerable<Boat> Boats(Race race);
		void SaveItem (TimingItem items);
		bool LastWriteSucceeded { get; } 
		DateTime LastWriteTime { get; } 
		string Name { get; } 
	}
}
