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
	// TODO - consider a non-string identifier 
	public interface IRepository<T>
	{
		IEnumerable<T> GetItems ();
		Func<bool> SaveItems (IEnumerable<T> items);
	}


	public class TimingItemRepositoryWeb : IRepository<TimingItem>
	{
		#region IRepository implementation

		public IEnumerable<TimingItem> GetItems ()
		{
			throw new NotImplementedException ();
		}

		public Func<bool> SaveItems (IEnumerable<TimingItem> items)
		{
			return () => true;
		}

		#endregion


	}

	// TODO - this is the dropbox repo - need a local file/db and FTP/HTTP version too. 
	public class TimingItemRepositoryDropbox : IRepository<TimingItem> 
	{
		public IEnumerable<TimingItem> GetItems ()
		{
			throw new NotImplementedException ();
		}

		public Func<bool> SaveItems (IEnumerable<TimingItem> items)
		{
			IDictionary<string, StringBuilder> dictionary = new Dictionary<string, StringBuilder> ();
			foreach (var item in items) 
			{
				var key = String.Format ("{0}.{1}", item.Race, item.Location);
				if (!dictionary.ContainsKey (key))
					dictionary.Add (key, new StringBuilder());
				dictionary[key].AppendLine(String.Format("{0}, {1}, \"{2}\", \"{3}\", {4}", item.StartNumber, item.Time.ToString("HH:mm:ss.fff"), item.Notes, item.GPS, item.Token));
			}

			return () => {
				bool rv = true;
				foreach (var kvp in dictionary)
				{
					if(!CreateFile (kvp.Key, kvp.Value))
						rv = false;
				}
				return rv;
			};
		}
			
		bool CreateFile (string stub, StringBuilder sb)
		{
			DBError error;

			// TODO - need a way to communicate this link back to base 
			var dbpath = DBPath.Root.ChildPath (stub + ".csv");

			if (DBFilesystem.SharedFilesystem == null) {
				Console.WriteLine ("either unconnected or dropbox is not authenticated.");
			} 
			else 
			{
				// TODO - OpenFile seems to reset the file on a new instance of the app (at least in the simulator) 
				//			var file = DBFilesystem.SharedFilesystem.CreateFile (dbpath, out error);
				var info = DBFilesystem.SharedFilesystem.FileInfoForPath(dbpath, out error);
				DBFile file;
				if(info != null)
					file = DBFilesystem.SharedFilesystem.OpenFile (dbpath, out error);
				else
					file = DBFilesystem.SharedFilesystem.CreateFile (dbpath, out error);
				if(error != null)
				{	
					Console.WriteLine("error: {0}, info: {1})", error.Code, info.ToString());
				}
				else 
				{
					file.WriteString (sb.ToString (), out error);
					return true;
				}
			}
			return false;
		}
	}

}

