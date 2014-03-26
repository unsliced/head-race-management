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
		public const string LocalFilenameFormat = "HeadRaceTiming/{0}.json";

		#region IRepository implementation

		public IEnumerable<TimingItem> GetItems (TimingItem item)
		{
			IEnumerable<TimingItem> rv;
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, string.Format(LocalFilenameFormat, item.FileNameStub));
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
			var filename = Path.Combine (documents, string.Format (LocalFilenameFormat, items.First ().FileNameStub));
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
}
