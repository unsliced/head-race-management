using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace TimingApp.DataLayer
{

	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public class TimingItemManager 
	{
		readonly IList<TimingItem> _list;
		readonly IRepository<TimingItem> _dbrepo;
		readonly IRepository<TimingItem> _jsonrepo;
		readonly IRepository<TimingItem> _httprepo;

		public void Reset ()
		{
			_list.Clear ();
		}

		Func<bool> _db;
		Func<bool> _web;
		Func<bool> _local;

		public TimingItemManager()
		{
			_dbrepo = new TimingItemRepositoryDropbox ();
			_jsonrepo = new TimingItemRepositoryLocal ();
			_httprepo = new TimingItemRepositoryWeb();
			_list = _jsonrepo.GetItems().ToList();
		}

		public IList<TimingItem> GetItems()
		{
			return _list;
		}

		public void SaveItem(TimingItem item)
		{
			_list.Add (item);
			_db = _dbrepo.SaveItems (_list);
			_web = _httprepo.SaveItems (_list);
			_local = _jsonrepo.SaveItems(_list);

			if (_db ())
				_db = null;
			if (_web ())
				_web =null;
			if (_local ())
				_local = null;
			// TODO - add a timer to retry if any are not null
			// TODO - keep a track to be able to report the status 
		}			
	}
}
