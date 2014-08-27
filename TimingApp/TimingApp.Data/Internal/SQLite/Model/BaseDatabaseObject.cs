using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;

namespace TimingApp.Data.Internal.SQLite.Model
{
	// todo: not clear if we need this to be INotifyPropertyChanged at this point, but could be useful if we're going to be background loading the data 
	abstract class BaseDatabaseObject<T, TOut> : BaseNotifyPropertyChanged
		where T : BaseDatabaseObject<T, TOut>, new() 
	{
		public BaseDatabaseObject() : base() { }

		public int Save()
		{
			return DatabaseUtils.DoAction<int, T>((SQLiteConnection connection) => connection.InsertOrReplace(this));
		}

		public virtual int Delete() 
		{
			return DatabaseUtils.DoAction<int, T>((SQLiteConnection connection) => connection.Delete(this));
		}

		protected static IList<T> GetAll()
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
