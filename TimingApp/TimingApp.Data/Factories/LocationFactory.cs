using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data.Factories
{

	public class LocationFactory : IFactory<ILocation>
	{
		string _name = string.Empty;
		string _token = string.Empty;

		public LocationFactory SetName(string name)
		{
			_name = name;
			return this;
		}

		public LocationFactory SetToken(string token)
		{
			_token = token;
			return this;
		}

		public ILocation Create()
		{
			return new Location(_name, _token); 
		}
	}
	
}
