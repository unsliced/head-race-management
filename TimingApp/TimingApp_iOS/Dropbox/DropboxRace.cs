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

		public string Code { get; set; } 
		public string Name { get; set; } 
		public DateTime Date { get; set; } 
		public DateTime BoatsUpdated { get; set; } 
		public DateTime DetailsUpdated { get; set; } 
		public string DataStoreID { get; set; } 

		public IEnumerable<IBoat> Boats { get ; set;  }
		public IEnumerable<ISequenceItem> Items { get ; set; } 
		public IEnumerable<ILocation> Locations { get ; set; }

		public DropboxRace() 
		{
			Code = string.Empty;
			Name = string.Empty;		
			Date = DateTime.MinValue;
			BoatsUpdated = DateTime.MinValue;
			DetailsUpdated = DateTime.MinValue;

			Boats = new List<IBoat>();
			Items = new List<ISequenceItem>();
			Locations = new List<ILocation>();
		}

	}
	
}
