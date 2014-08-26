using System;
using Xamarin.Forms;
using SQLite.Net;
using TimingApp.Portable.Database;

namespace TimingApp.Portable.Database
{
	public class TimingDatabase
	{
		readonly SQLiteConnection _database;

		public TimingDatabase() {
			_database = DependencyService.Get<ISQLite>().GetConnection ();
		}

		public SQLiteConnection Connection { get { return _database; } }
	}
}

