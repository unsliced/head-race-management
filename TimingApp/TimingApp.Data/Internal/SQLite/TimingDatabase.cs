using System;
using Xamarin.Forms;
using SQLite.Net;
using TimingApp.Data.Interfaces;

namespace TimingApp.Data.Internal.SQLite 
{
	class TimingDatabase
	{
		readonly SQLiteConnection _database;

		public TimingDatabase() {
			_database = DependencyService.Get<ISQLite>().GetConnection ();
		}

		public SQLiteConnection Connection { get { return _database; } }
	}
}

