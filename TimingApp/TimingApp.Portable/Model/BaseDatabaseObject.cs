using System;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.ComponentModel;
using System.Collections.Generic;
using TimingApp.Portable.Database;

namespace TimingApp.Portable.Model
{
	// TODO: this should probably be internal, but let's not get too hung up on it at this point 

	public abstract class BaseDatabaseObject<T> : INotifyPropertyChanged
		where T : BaseDatabaseObject<T>, new() 
	{
		#region INotifyPropertyChanged 

		// blatantly nabbed from http://stackoverflow.com/a/1316417/2902 

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T2>(ref T2 field, T2 value, string propertyName)
		{
			if (EqualityComparer<T2>.Default.Equals(field, value)) return false;
			field = value;
			// todo: consider saving here 
			OnPropertyChanged(propertyName);
			return true;
		}

		#endregion 

		public BaseDatabaseObject() { }

		public int Save()
		{
			return DatabaseUtils.DoAction<int, T>((SQLiteConnection connection) => connection.InsertOrReplace(this));
		}

//		public abstract int Delete();

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

	// TODO: this should probably be internal, but let's not get too hung up on it at this point 
	
}
