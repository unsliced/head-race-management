using System;
using DropBoxSync.iOS;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace TimingApp_iOS.DropboxBoat 
{
	public class DropboxRace : IRace
	{
		IEnumerable<string> _locationNames;

		public string Code { get; set; } 
		public string Name { get; set; } 
		public DateTime Date { get; set; } 
		public DateTime BoatsUpdated { get; set; } 
		public DateTime DetailsUpdated { get; set; } 
		public DBDatastore Racestore { get; set; } 
		// hack - hide these 
		public IDictionary<int,DBRecord> BoatRecords = new Dictionary<int, DBRecord> ();
		public IDictionary<int,IBoat> BoatDictionary = new Dictionary<int, IBoat> ();

		public IEnumerable<IBoat> Boats { get { return BoatDictionary.Values.ToList(); } }

		public IEnumerable<ILocation> Locations { get { throw new NotImplementedException(); } }

		public IEnumerable<string> LocationNames { get { return _locationNames; } } 

		public DropboxRace() 
		{
			Code = string.Empty;
			Name = string.Empty;		
			Date = DateTime.MinValue;
			BoatsUpdated = DateTime.MinValue;
			DetailsUpdated = DateTime.MinValue;
			Racestore = null; 
			_locationNames = new List<string>();
		}

		public void SetLocationNames(IEnumerable<string> locationNames)
		{
			_locationNames = locationNames;
		}
	}
	
}
