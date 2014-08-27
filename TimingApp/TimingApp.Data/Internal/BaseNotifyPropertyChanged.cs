using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace TimingApp.Data.Internal
{
	abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
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
	}
}
