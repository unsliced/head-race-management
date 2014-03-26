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
		IEnumerable<T> GetItems (T sample);
		Func<bool> SaveItems (IEnumerable<T> items);
	}

	// TODO - this is the dropbox repo - need a local file/db and FTP/HTTP version too. 

}
