using System;
using SQLite.Net;

namespace TimingApp.Data.Interfaces
{
	public interface ISQLite {
		SQLiteConnection GetConnection();
	}
	
}
