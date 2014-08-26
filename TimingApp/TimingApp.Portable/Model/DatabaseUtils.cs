using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Portable.Database;

namespace TimingApp.Portable.Model
{
	// TODO: this should probably be internal, but let's not get too hung up on it at this point 
	static class DatabaseUtils
	{
		// HACK: this should be false for all bar occasional runs 					
		static bool StartFresh = true;

		public static T DoAction<T, T1>(this Func<SQLiteConnection, T> action)
		{

			using(var connection = new TimingDatabase().Connection)
			{
				if(StartFresh)
				{
					connection.DropTable<TimingItem>();
					connection.DropTable<Boat>();
					connection.DropTable<Race>();
					StartFresh = false; // otherwise it would be continually dropping the tables ... 

					connection.CreateTable<TimingItem>();
					connection.CreateTable<Boat>();
					connection.CreateTable<Race>();

					connection.Execute ("insert into Races (_code, _name) values (?, ?)", "adhoc", "AdHoc Race");
					connection.Execute ("insert into Races (_code, _name) values (?, ?)", "vh14", "Vets Head 2014");

					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 1, "Thames RC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 2, "Broxbourne RC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 3, "C N Betulo (ES)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 4, "Bristol Ariel/Tyne RC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 5, "Quintin BC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 6, "Parr's Priory");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 7, "STCRC (DE)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 8, "Cercla Nautique de France (FR)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 9, "Kar & Zv De Hoop (NL)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 10, "Agecroft");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 11, "York City");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 12, "Corgeno Sc (IT)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 13, "RCR Alicante (ES)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 14, "Royal Maas YC (NL)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 15, "Tyrian Club/Thames RC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 16, "Canottieri Ravenna (IT) [Lunghi]");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 17, "Cygnet RC");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 18, "Canottieri Ravenna (IT) [Pelligra]");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 19, "S C Armida (IT)");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 20, "Crabtree");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "vh14", 21, "Monmouth RC");

					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 1, "Crew 1");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 2, "Crew 2");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 3, "Crew 3");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 4, "Crew 4");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 5, "Crew 5");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 6, "Crew 6");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 7, "Crew 7");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 8, "Crew 8");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 9, "Crew 9");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 10, "Crew 10");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 11, "Crew 11");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 12, "Crew 12");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 13, "Crew 13");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 14, "Crew 14");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 15, "Crew 15");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 16, "Crew 16");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 17, "Crew 17");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 18, "Crew 18");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 19, "Crew 19");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 20, "Crew 20");
					connection.Execute("insert into Boats (_race, _number, _name) values (?, ?, ?)", "adhoc", 21, "Crew 21");
				}

				connection.CreateTable<T1>();

				var result = action(connection);
				return result;
			}
		}

		public static IEnumerable<T> GetAll<T>() where T : new() 
		{
			return DatabaseUtils.DoAction<IList<T>, T>((SQLiteConnection connection) => 
				{
					var t = connection.Table<T>();
					var rv = new List<T>();
					foreach(var q in t)
						rv.Add(q);

					return rv;
				});
		}	
	}
}
