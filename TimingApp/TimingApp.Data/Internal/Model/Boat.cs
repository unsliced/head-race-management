using System;
using TimingApp.Data.Interfaces;
using System.Collections.Generic;
using TimingApp.Data.Internal;
using Xamarin.Forms;

namespace TimingApp.Data.Internal.Model
{
	class Boat : BaseNotifyPropertyChanged, IBoat 
	{
		readonly string _name;
		readonly int _number;
		readonly string _category;

		public Boat(int number, string name, string category, bool end)
		{
			_number = number;
			_name = name;
			_category = category;
			_end = end;
			if(number < 0)
				BackgroundColour = Color.Red;
		}

		#region IEquatable implementation

		public bool Equals(IBoat other)
		{
			return Number == other.Number;
		}

		#endregion

		#region IBoat implementation

		public int Number { get { return _number; } }
		public string Name { get { return _name; } }
		public string Category { get { return _category; } }

		bool _seen = false;
		public bool Seen { 
			get 
			{ 
				return _seen; 
			} 
			set 
			{ 
				BackgroundColour = value ? Color.Gray : Color.White ;
				SetField(ref _seen, value, "Seen"); 
			} 
		} 

		bool _end = false;
		public bool End { 
			get 
			{ 
				return _end; 
			} 
			set 
			{ 
				SetField(ref _end, value, "End"); 
			} 
		} 


		Color _backgroundColour;
		public Color BackgroundColour 
		{ 
			get { 
				return _backgroundColour; 
			} 
			set 
			{ 
				SetField(ref _backgroundColour, value, "BackgroundColour"); 
			}
		}

		public string PrettyName {
			get
			{
				return string.Format("{0} / {1} / {2}", Number, Name, Category);
			}
		}


		#endregion

	}	

}
