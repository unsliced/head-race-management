using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace TimingApp.DataLayer
{
	public class TimingItemRepository 
	{
		static TimingItemRepository _me;

		static TimingItemRepository()
		{
			_me = new TimingItemRepository ();
		}

		public static IList<TimingItem> GetItems (string id)
		{
			return new List<TimingItem> ();
		}

		public static int SaveItems (string id, IEnumerable<TimingItem> item)
		{
			return item.Count ();
		}
	}

}

