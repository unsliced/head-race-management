using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;
using System.Collections.ObjectModel;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.SQLite;
using TimingApp.Data.Internal.Model;
using System.Diagnostics;

namespace TimingApp.Data
{
	public class TimingItemManager 
	{
		IList<IBoat> _keepUnfinished;

		readonly ILocation _location;
		readonly IList<IRepository> _repos;
		string _filter = string.Empty;

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

		IBoat UnidentifiedBoat { 
			get { 
				int lowest = _location.SequenceItems.Count == 0 ? 0 : _location.SequenceItems.Min(x => x.Boat.Number);
				lowest--;
				return new Boat(lowest, "Unidentified", String.Empty, false); 
			} 
		}

		public void Unidentified()
		{
			ISequenceItem item = new SequenceItem(UnidentifiedBoat, DateTime.Now, string.Empty);
			_location.SequenceItems.Add(item);
			Finished.Clear();
			_location.SequenceItems.OrderByDescending(i => i.TimeStamp).ForEach(Finished.Add);
		}

		// todo: filter this (e.g. hidden) 

		public ObservableCollection<IBoat> Unfinished { get; private set; }
		public ObservableCollection<ISequenceItem> Finished { get; private set; }

		public void SaveBoat(IEnumerable<Tuple<IBoat, DateTime, string>> list)
		{
			foreach(var tuple in list)
			{
				var boat = tuple.Item1;
				var time = tuple.Item2;
				var notes = tuple.Item3;

				ISequenceItem item = new SequenceItem(boat == null ? UnidentifiedBoat : boat, time, notes);
				_location.SequenceItems.Add(item);

				var sw = new Stopwatch();
				sw.Start();
				// note: for now this has to happen first, otherwise the visible time is not going to be populated ahead of being displayed in the master panel binding 
				_repos.ForEach(r => r.LogATime(_location, item));
				sw.Stop();
				ReportStopwatch(sw, boat.Number.ToString());


				_keepUnfinished.Remove(boat);
				Unfinished.Remove(boat);

//				if(boat.Number < 0)
//					Unfinished.Insert(0, UnidentifiedBoat);
			}
			RefreshObservable();
		}

		public void Filter(string text)
		{
			_filter = text.ToLowerInvariant();
			RefreshObservable();
		}

		void RefreshObservable()
		{
			var sw = new Stopwatch();

			sw.Start();

			Finished.Clear();
			_location.SequenceItems.OrderByDescending(i => i.TimeStamp).ForEach(Finished.Add);

			Unfinished.Clear();
//			Unfinished.Insert(0, UnidentifiedBoat);

			foreach(var u in 
				_keepUnfinished
				.Where(b => b.Number < 0 || string.IsNullOrEmpty(_filter) || b.PrettyName.ToLowerInvariant().Contains(_filter))
				.OrderBy(b => b.End ? 1 : 0)
				.ThenBy(b => b.Number))

			{
				Unfinished.Add(u);
//				if(Unfinished.Count > 5)
//					break;
			}
			sw.Stop();
			ReportStopwatch(sw, _filter);
		
			// TODO - add a timer to retry if any are not null
			// TODO - keep a track to be able to report the status 
		}

		void ReportStopwatch(Stopwatch sw, string arg)
		{
			// Get the elapsed time as a TimeSpan value.
			TimeSpan ts = sw.Elapsed;

			// Format and display the TimeSpan value. 
			string elapsedTime = String.Format("{4} - {0:00}:{1:00}:{2:00}.{3:000}",
				ts.Hours, ts.Minutes, ts.Seconds,
				ts.Milliseconds, arg );

			Debug.WriteLine(elapsedTime);

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

		// todo: put a ticking clock/summary (e.g. number of finishers, progress bar, etc.) into the title bar 
		// idea: show the status of the saved 
		public string Title { get { return "timing app"; } }
	}

	public class SaveStatus
	{
		public bool Success { get; set; }
		public string Repo { get; set; } 
		public DateTime WriteTime { get; set; } 
	}
}
