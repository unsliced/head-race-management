using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;
using System.Collections.Generic;

namespace TimingApp.Data.Factories
{
	public class BoatFactory : IFactory<IBoat> 
	{
		int _number;
		string _category; 
		string _name;

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

		public IBoat Create() {
			return new Boat(_number, _name, _category);
		}
	}		
}
