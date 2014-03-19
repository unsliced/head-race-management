using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;
using DropBoxSync.iOS;
using System.Text;

namespace TimingApp.DataLayer
{
	// TODO - consider a non-string identifier 
	public interface IRepository<T>
	{
		IList<T> GetItems (string identifier);
		int SaveItems (string identifier, IEnumerable<T> items);
	}

	// TODO - this is the dropbox repo - need a local file/db and FTP/HTTP version too. 
	public class TimingItemRepositoryDropbox : IRepository<TimingItem> 
	{

		public IList<TimingItem> GetItems (string id)
		{
			return new List<TimingItem> ();
		}

		public int SaveItems (string id, IEnumerable<TimingItem> items)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var item in items) 
			{
				sb.AppendLine(String.Format("{0}, {1}, \"{2}\"", item.StartNumber, item.Time.ToString("HH:mm:ss.fff"), item.Notes));
			}

			CreateFile (sb);

			return items.Count ();
		}

		static void CreateFile (StringBuilder sb)
		{
			DBError error;

			// TODO - need a better file name 
			// TODO - need a way to communicate this link back to base 
			var dbpath = DBPath.Root.ChildPath ("hello.txt");
			if (DBFilesystem.SharedFilesystem == null) {
				Console.WriteLine ("either unconnected or dropbox is not authenticated.");
			} else {
				// TODO - OpenFile seems to reset the file on a new instance of the app (at least in the simulator) 
				//			var file = DBFilesystem.SharedFilesystem.CreateFile (dbpath, out error);
				var file = DBFilesystem.SharedFilesystem.OpenFile (dbpath, out error);
				file.WriteString (sb.ToString (), out error);
			}
		}
	}

}

