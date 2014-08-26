using System;
using TimingApp.Portable.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;

namespace TimingApp.Portable.DataLayer
{

	public class TimingItemRepositoryDatabase : IRepository
	{
		bool _lastWriteSucceeded;
		DateTime _lastWriteTime;

		public TimingItemRepositoryDatabase() {

		} 

		#region IRepository implementation

		public IEnumerable<TimingItem> TimingItems (Race race)
		{
			return DatabaseUtils.GetAll<TimingItem> ().Where (i => i.Race == race.Code);
		}

		public IEnumerable<Boat> Boats (Race race)
		{
			return DatabaseUtils.GetAll<Boat> ().Where (b => b.Race == race.Code);
		}

		public void SaveItem (TimingItem item)
		{
			int rv = DatabaseUtils.DoAction<int, TimingItem>((SQLiteConnection connection) => connection.InsertOrReplace(item));
			_lastWriteSucceeded = rv == 1;
			if (rv >= 1)
				_lastWriteTime = DateTime.Now;
		}

		public IEnumerable<Race> AllRaces { get { return DatabaseUtils.GetAll<Race> (); } }

		public bool LastWriteSucceeded { get { return _lastWriteSucceeded; } }

		public DateTime LastWriteTime { get { return _lastWriteTime; } }

		public string Name { get { return "Database"; } }

		#endregion
	}

	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public class TimingItemManager 
	{
		public string Location { get; set; } 
		public string Token { get; set; } 
		public Race Race { get; set; } 

		readonly IRepository _databaserepo;
		readonly IList<IRepository> _repos;

		public TimingItemManager()
		{
			_databaserepo = new TimingItemRepositoryDatabase();
			_repos = new List<IRepository> { _databaserepo };

			// todo: retrieve existing file from the folder 
			// _repos.Add(new TimingItemRepositoryDropbox ());
			// _repos.Add(new TimingItemRepositoryLocal ());
			// _repos.Add(new TimingItemRepositoryWeb());
			// _list = _jsonrepo.GetItems(new TimingItem(race, location, string.Empty, token, -1, DateTime.MinValue, string.Empty)).ToList();
		}

		public IEnumerable<Race> Races { get { return _databaserepo.AllRaces; } } 
		public IEnumerable<TimingItem> Items { get { return _databaserepo.TimingItems (Race); } } 
		public IEnumerable<Boat> Boats { get { return _databaserepo.Boats(Race); } } 

		// todo: filter this (e.g. hidden) 
		public IEnumerable<Boat> Unfinished { 
			get 
			{ 
				// fixme: need this to reflect the boats that don't have a timing item 
				return Boats.Where (b => !Items.Any (i => i.Boat.Number == b.Number));
			} 
		}
		public IEnumerable<TimingItem> Finished { get { return Items.Where(i => i.Boat != null); } }


		public void SaveItem(TimingItem item)
		{
			_repos.ForEach (r => r.SaveItem (item));

			// TODO - add a timer to retry if any are not null
			// TODO - keep a track to be able to report the status 
		}

		public IEnumerable<SaveStatus> Status {
			get 
			{
				return _repos.Select (r => new SaveStatus {
					Success = r.LastWriteSucceeded,
					Repo = r.Name,
					WriteTime = r.LastWriteTime
				}); 
			}
		}

	}
}
