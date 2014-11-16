using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;
using System.Collections.ObjectModel;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.SQLite;
using TimingApp.Data.Enums;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data
{
	public class TimingItemManager 
	{
		IList<IBoat> _keepFinished;
		IList<IBoat> _keepUnfinished;

		readonly ObservableCollection<IBoat> _finished;
		readonly ObservableCollection<IBoat> _unfinished;

		readonly IRepository _mainrepo;
		readonly IList<IRepository> _repos;
		readonly IRace _race;

		public TimingItemManager(IRepository mainrepo, IList<IRepository> otherRepos, 
			string racecode, string token, Endpoint location, bool sequence)
		{
			// todo : import json defined crew summaries into the database 
			// var dbrepo = new TimingItemRepositoryDatabase(racecode, token, location, sequence);
			_race = racecode;
			_mainrepo = mainrepo;
			_repos = new List<IRepository> (otherRepos);
			_repos.Add(mainrepo);

			_keepUnfinished = new List<IBoat>();
			_keepFinished = new List<IBoat>();
			foreach(var boat in _race.Boats)
			{
				if(boat.TimeStamp >= DateTime.MinValue)
					_keepFinished.Add(boat);
				else
					_keepUnfinished.Add(boat);
			}
			_finished = new ObservableCollection<IBoat>(_keepFinished);
			_unfinished = new ObservableCollection<IBoat>(_keepUnfinished);
			_unfinished.Add(UnidentifiedBoat);

			// todo: retrieve existing file from the folder 
			// _repos.Add(new TimingItemRepositoryDropbox ());
			// _repos.Add(new TimingItemRepositoryLocal ());
			// _repos.Add(new TimingItemRepositoryWeb());
			// _list = _jsonrepo.GetItems(new TimingItem(race, location, string.Empty, token, -1, DateTime.MinValue, string.Empty)).ToList();
		}

		IBoat UnidentifiedBoat { get { return new Boat(-1, "Unidentified", String.Empty, _race, _location); } }

		public static IDictionary<string, string> RaceCodes { get { return TimingItemRepositoryDatabase.RaceCodes; } } 

		// todo: filter this (e.g. hidden) 

		public ObservableCollection<IBoat> Unfinished { get { return _unfinished; } }
		public ObservableCollection<IBoat> Finished { get { return _finished; } }

		public void SaveBoat(IBoat boat, DateTime time, string notes)
		{
			// fixme: if the boat is null, then it should be logged against the location's unidentified list 

			// note: for now this has to happen first, otherwise the visible time is not going to be populated ahead of being displayed in the master panel binding 
			_repos.ForEach (r => r.LogATime(_location, boat, time, notes));

			Finished.Clear();
			Unfinished.Clear();

			_keepUnfinished.Remove(boat);
			if(boat.Number > 0)
				_unfinished.Add(UnidentifiedBoat);

			_keepFinished.Add(boat);

			_keepFinished.ForEach(Finished.Add);
			_keepUnfinished.ForEach(Unfinished.Add);

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
