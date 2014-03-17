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

		public TimingItemManager(string id)
		{
			_id = id;
			_list = TimingItemRepository.GetItems(_id);
		}

		public IList<TimingItem> GetItems()
		{
			return _list;
		}

		public int SaveItem(TimingItem item)
		{
			_list.Add (item);
			return TimingItemRepository.SaveItems(_id, _list);
		}			
	}
}
