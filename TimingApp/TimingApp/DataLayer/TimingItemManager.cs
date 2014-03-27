using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

		public TimingItemManager(string race, string location, string token)
		{
			_dbrepo = new TimingItemRepositoryDropbox ();
			_jsonrepo = new TimingItemRepositoryLocal ();
			_httprepo = new TimingItemRepositoryWeb();
			_list = _jsonrepo.GetItems(new TimingItem(race, location, string.Empty, token, -1, DateTime.MinValue, string.Empty)).ToList();
		}

		public IList<TimingItem> GetItems()
		{
			return _list;
		}

		void SaveItems (string endpoint, Func<bool> func, IRepository<TimingItem> repo)
		{
			if (func != null)
				return;
			func = repo.SaveItems (_list);
			try {
				if (func ())
					func = null;
				else
				{
					// this means it has failed, so we should retry after an elapsed timer 
					Console.WriteLine("failed to write to the endpoint. need to try again");
				}
			}
			catch (Exception ex) {
				Console.WriteLine ("Exception in writing to {0}: {1}", endpoint, ex.Message);
			}
		}

		public void SaveItem(TimingItem item)
		{
			if(item != null)
				_list.Add (item);
			SaveItems ("Local", _local, _jsonrepo);
			SaveItems ("DropBox", _db, _dbrepo);
			SaveItems ("Web", _web, _httprepo); 

			// TODO - add a timer to retry if any are not null
			// TODO - keep a track to be able to report the status 
		}

		public IEnumerable<SaveStatus> Status {
			get 
			{
				return 
					from repo in new List<IRepository<TimingItem>>{ _dbrepo, _httprepo, _jsonrepo }
				select new SaveStatus { Success = repo.LastWriteSucceeded, Repo = repo.Name, WriteTime = repo.LastWriteTime};

			}
		}

	}
}
