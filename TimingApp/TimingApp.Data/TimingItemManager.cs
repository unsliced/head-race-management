using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;
using System.Collections.ObjectModel;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.SQLite;
using TimingApp.Data.Enums;

namespace TimingApp.Data
{
	public class TimingItemManager 
	{
		IList<IBoat> _keepFinished;
		IList<IBoat> _keepUnfinished;

		readonly ObservableCollection<IBoat> _finished;
		readonly ObservableCollection<IBoat> _unfinished;

		readonly IRepository _databaserepo;
		readonly IList<IRepository> _repos;
		readonly ILocation _location;
		readonly IRace _race;

		public TimingItemManager(string racecode, string token, Endpoint location, bool sequence)
		{
			var dbrepo = new TimingItemRepositoryDatabase(racecode, token, location, sequence);
			_location = dbrepo.Location;
			_race = dbrepo.Race;
			_databaserepo = dbrepo;
			_repos = new List<IRepository> { _databaserepo };
			_keepUnfinished = new List<IBoat>();
			_keepFinished = new List<IBoat>();
			foreach(var boat in _race.Boats)
			{
				if(boat.Time != null && boat.Time.Any(t => t.Location == _location))
					_keepFinished.Add(boat);
				else
					_keepUnfinished.Add(boat);
			}
			_finished = new ObservableCollection<IBoat>(_keepFinished);
			_unfinished = new ObservableCollection<IBoat>(_keepUnfinished);

			// todo: retrieve existing file from the folder 
			// _repos.Add(new TimingItemRepositoryDropbox ());
			// _repos.Add(new TimingItemRepositoryLocal ());
			// _repos.Add(new TimingItemRepositoryWeb());
			// _list = _jsonrepo.GetItems(new TimingItem(race, location, string.Empty, token, -1, DateTime.MinValue, string.Empty)).ToList();
		}

		public static IDictionary<string, string> RaceCodes { get { return TimingItemRepositoryDatabase.RaceCodes; } } 

		// todo: filter this (e.g. hidden) 
		public ObservableCollection<IBoat> Unfinished { get { return _unfinished; } }
		public ObservableCollection<IBoat> Finished { get { return _finished; } }

		public void SaveBoat(IBoat boat, DateTime time, string notes)
		{
			Finished.Clear();
			Unfinished.Clear();

			_keepUnfinished.Remove(boat);
			_keepFinished.Add(boat);

			_keepFinished.ForEach(Finished.Add);
			_keepUnfinished.ForEach(Unfinished.Add);

			_repos.ForEach (r => r.LogATime(_location, boat, time, notes));

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

	public class SaveStatus
	{
		public bool Success { get; set; }
		public string Repo { get; set; } 
		public DateTime WriteTime { get; set; } 
	}
}
