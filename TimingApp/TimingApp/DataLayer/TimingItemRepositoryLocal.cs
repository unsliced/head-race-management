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

	public class TimingItemRepositoryLocal : IRepository<TimingItem>
	{
		const string LocalFilename = "HeadRaceTiming/Items.json";
		#region IRepository implementation

		public IEnumerable<TimingItem> GetItems ()
		{
			IEnumerable<TimingItem> rv;
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, LocalFilename);
			if (!File.Exists (filename))
				rv = null;
			else
			{
				var text = File.ReadAllText(filename);
				var objects = JsonConvert.DeserializeObject<List<TimingItem>>(text);
				// TODO - better handle pre-existing and the filter 
				rv = objects as IEnumerable<TimingItem>;
			}
			return rv ?? new List<TimingItem> ();
		}

		public Func<bool> SaveItems (IEnumerable<TimingItem> items)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, LocalFilename);
			string json = JsonConvert.SerializeObject (items);

			if(!Directory.Exists(Path.GetDirectoryName(filename)))
				Directory.CreateDirectory (Path.GetDirectoryName (filename));

			return () => {
				File.WriteAllText (filename, json);
				return true;
			}; 
		}

		#endregion

	}

	// TODO - this is the dropbox repo - need a local file/db and FTP/HTTP version too. 

}
