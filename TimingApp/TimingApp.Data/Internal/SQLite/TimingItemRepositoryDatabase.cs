using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SQLite.Net;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.SQLite;
using TimingApp.Data.Internal.SQLite.Model;
using TimingApp.Data.Internal.Model;

namespace TimingApp.Data.Internal.SQLite
{
	// todo - are we even going to be using the local database? 
	class TimingItemRepositoryDatabase : IRepository
	{
		bool _lastWriteSucceeded;
		DateTime _lastWriteTime;
		readonly ILocation _location;
		readonly IRace _race;

		public TimingItemRepositoryDatabase(string racecode, string locationcode, string locationtoken) 
		{
			Race race = DatabaseUtils.GetAll<DbRace>().Where(r => r.Code == racecode).First().As(); 
			ILocation location = new Location(locationcode, locationtoken, new List<ISequenceItem>());

			IList<Boat> boats = DatabaseUtils.GetAll<DbBoat>().Where(b => b.RaceCode == racecode).Select(b => b.As(race, location)).ToList();
			//boats.ForEach(race.AddBoat);

			//race.AddLocation(location);
			var items = new List<DbTimingItem>(); // DatabaseUtils.GetAll<DbTimingItem>().Where(i => i.Race == racecode && i.Location == location.Endpoint.ToString() && i.Token == location.Code);

			foreach(DbTimingItem item in items)
			{
				// fixme: if the boat is null (startnumber is negative), then it should be logged against the location's unidentified list 

				Boat boat = boats.Where(b => b.Number == item.StartNumber).First();
				SequenceItem ts = item.As(boat, location);
				//boat.AddTime(location, ts);
			}

			_location = location;
			_race = race;
		} 



		#region IRepository implementation

		public void LogATime(ILocation location, ISequenceItem item)
		{
			// fixme: if the boat is null, then it should be logged against the location's unidentified list 

			//var item = DbTimingItem.Create(boat.Race, location, boat, time, notes);
			//var stamp = item.As(boat, location);
			//boat.Times.Add(location, stamp);
//			int wr = item.Save();
//			_lastWriteSucceeded = wr == 1;
//			if(_lastWriteSucceeded)
//				_lastWriteTime = DateTime.Now;
		}

		public bool LastWriteSucceeded { get { return _lastWriteSucceeded; } }

		public DateTime LastWriteTime { get { return _lastWriteTime; } }

		public string Name { get { return "Database"; } }

		public ILocation Location { get { return _location; } }
		public IRace Race { get { return _race; } } 


		public event EventHandler RaceListUpdated;

		public void Update()
		{
			throw new NotImplementedException();
		}

		public void AddRaceCode(string code)
		{
			throw new NotImplementedException();
		}

		public void SetRace(string race) 
		{
			throw new NotImplementedException();
		}


		public IEnumerable<IRace> RaceList {
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ILocation> LocationList {
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ISequenceItem> ItemList(string name, string code)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IBoat> BoatList {
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion
	}
	
}
