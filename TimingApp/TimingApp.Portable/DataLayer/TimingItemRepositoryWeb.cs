using System;
using TimingApp.Portable.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace TimingApp.Portable.DataLayer
{
	public class TimingItemRepositoryWeb : IRepository<TimingItem>
	{
		bool _lastWrite = false;
		DateTime _lastWriteTime = DateTime.MinValue;

		#region IRepository implementation

		public IEnumerable<TimingItem> GetItems (TimingItem item)
		{
			throw new NotImplementedException ();
		}

		public Func<bool> SaveItems (IEnumerable<TimingItem> items)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, string.Format (TimingItemRepositoryLocal.LocalFilenameFormat, items.First ().FileNameStub));

			return () => {
				var client = new WebClient ();
				// chris - another horrible cludge on the filename 
				client.UploadFileCompleted += (sender, e) => 
				{ 
					if(e.Result != null)
						Console.WriteLine("\nResponse Received.The contents of the file uploaded are:\n{0}", System.Text.Encoding.ASCII.GetString(e.Result));
					if(e.Error != null)
						Console.WriteLine("\nError received:\n{0}", e.Error.Message);

					_lastWrite = e.Error == null;
					if(_lastWrite) 
						_lastWriteTime = DateTime.Now;
				};
				client.UploadFileAsync (new Uri("http://unsliced.webfactional.com/hrm/VetsHead2014/update.php"), filename);
				return true;
			}; 
		}

		// chris - definite candidates for a base class. 
		public bool LastWriteSucceeded { get { return _lastWrite; } }
		public DateTime LastWriteTime { get { return _lastWriteTime; } }
		public string Name { get { return "Web"; } }

		#endregion

	}
}
