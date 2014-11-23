using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;
using System.Collections.ObjectModel;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.SQLite;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data
{
	public class TimingItemManager 
	{
		IList<IBoat> _keepUnfinished;

		readonly ILocation _location;
		readonly IList<IRepository> _repos;

		public TimingItemManager(IList<IRepository> repos, 
			ILocation location, 
			IEnumerable<IBoat> boats)
		{
			_repos = new List<IRepository> (repos);
			_location = location;

			_keepUnfinished = new List<IBoat>(boats);

			Finished = new ObservableCollection<ISequenceItem>(_location.SequenceItems);
			foreach(var alreadyFinished in Finished)
				_keepUnfinished.Remove(alreadyFinished.Boat);

			Unfinished = new ObservableCollection<IBoat>(_keepUnfinished);

			RefreshObservable();

			// todo: retrieve existing file from the folder 
			// _repos.Add(new TimingItemRepositoryDropbox ());
			// _repos.Add(new TimingItemRepositoryLocal ());
			// _repos.Add(new TimingItemRepositoryWeb());
			// _list = _jsonrepo.GetItems(new TimingItem(race, location, string.Empty, token, -1, DateTime.MinValue, string.Empty)).ToList();
		}

		IBoat UnidentifiedBoat { get { return new Boat(-1, "Unidentified", String.Empty); } }

		// todo: filter this (e.g. hidden) 

		public ObservableCollection<IBoat> Unfinished { get; private set; }
		public ObservableCollection<ISequenceItem> Finished { get; private set; }

		public void SaveBoat(IBoat boat, DateTime time, string notes)
		{
			ISequenceItem item = new SequenceItem(boat == null ? UnidentifiedBoat : boat, time, notes);
			_location.SequenceItems.Add(item);

			// note: for now this has to happen first, otherwise the visible time is not going to be populated ahead of being displayed in the master panel binding 
			_repos.ForEach(r => r.LogATime(_location, item));

			_keepUnfinished.Remove(boat);

			RefreshObservable();
		}

		void RefreshObservable()
		{
			Finished.Clear();
			Unfinished.Clear();
			Unfinished.Add(UnidentifiedBoat);
			_location.SequenceItems.OrderByDescending(i => i.TimeStamp).ForEach(Finished.Add);
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
