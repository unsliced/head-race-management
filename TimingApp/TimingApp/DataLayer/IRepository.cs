using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;
using DropBoxSync.iOS;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace TimingApp.DataLayer
{
	public interface IRepository<T>
	{
		IEnumerable<T> GetItems (T sample);
		Func<bool> SaveItems (IEnumerable<T> items);
		bool LastWriteSucceeded { get; } 
		DateTime LastWriteTime { get; } 
		string Name { get; } 
	}
}
