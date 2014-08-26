using System;
using System.IO;
using SQLite.Net;
using Xamarin.Forms;
using TimingApp;
using TimingApp.Portable.Database;

// it's not immediately clear, but this needs to be outside the namespace definition 
[assembly: Dependency (typeof (SQLite_iOS))]
namespace TimingApp
{
	// based on the sample code from http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/databases/
	public class SQLite_iOS : ISQLite {
		public SQLite_iOS () {}
		public SQLiteConnection GetConnection ()
		{
			var sqliteFilename = "QuizSQLite.db3";
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			// Create the connection
			var plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			var conn = new SQLite.Net.SQLiteConnection(plat, path);
			// Return the database connection
			return conn;
		}	
	}
}

