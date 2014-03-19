using System;
using TimingApp.Model;
using System.Collections.Generic;

namespace TimingApp.DataLayer
{

	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public class TimingItemManager 
	{
		readonly string _id;
		readonly IList<TimingItem> _list;
		readonly IRepository<TimingItem> _repo;

		public TimingItemManager(string id)
		{
			_id = id;
			_repo = new TimingItemRepositoryDropbox ();
			_list = _repo.GetItems(_id);
		}

		public IList<TimingItem> GetItems()
		{
			return _list;
		}

		public int SaveItem(TimingItem item)
		{
			_list.Add (item);
			return _repo.SaveItems(_id, _list);
		}			
	}
}
