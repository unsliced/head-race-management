using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;
using System.Collections.Generic;

namespace TimingApp.Data.Factories
{
	public class BoatFactory : IFactory<IBoat> 
	{
		int _number = -1;
		string _category = string.Empty; 
		string _name = string.Empty;
		bool _end = false;

		public BoatFactory SetNumber(int number)
		{
			_number = number;
			return this;
		}

		public BoatFactory SetName(string name)
		{
			_name = name;
			return this;
		}

		public BoatFactory SetCategory(string category)
		{
			_category = category;
			return this;
		}

		public BoatFactory SetScratched(bool end)
		{
			_end = end;
			return this; 
		}

		public IBoat Create() {
			return new Boat(_number, _name, _category, _end);
		}
	}	
}
