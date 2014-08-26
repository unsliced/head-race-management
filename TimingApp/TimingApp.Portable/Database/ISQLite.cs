using System;
using SQLite.Net;

namespace TimingApp.Portable.Database
{
	public interface ISQLite {
		SQLiteConnection GetConnection();
	}
	
}
