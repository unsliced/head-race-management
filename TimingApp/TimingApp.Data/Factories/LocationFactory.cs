using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;
using System.Collections.Generic;

namespace TimingApp.Data.Factories
{

	public class LocationFactory : IFactory<ILocation>
	{
		string _name = string.Empty;
		string _token = string.Empty;
		IEnumerable<ISequenceItem> _items;

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

		public LocationFactory SetItems(IRepository repo)
		{	
			_items = repo.ItemList(_name, _token);

			return this;
		}

		public ILocation Create()
		{
			return new Location(_name, _token, _items); 
		}
	}
	
}
