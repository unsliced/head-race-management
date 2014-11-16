using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TimingApp_iOS
{
	// todo - make this JsonEntry into a shared definition with the draw app 
	[DataContract]
	public class JsonEntry
	{
		[JsonProperty]
		public int StartNumber { get ; set; } 
		[JsonProperty]
		public string Name { get ; set; } 
		[JsonProperty]
		public string Category { get ; set; } 
		[JsonProperty]
		public string ClubIndex { get ; set; } 
	}

	[DataContract]
	public class JsonLocation
	{
		[JsonProperty]
		public string Location { get; set; } 
	}

	[DataContract]
	public class JsonDetails
	{
		[JsonProperty]
		public string Name { get; set; } 
		[JsonProperty]
		public string Date { get; set; } 
		[JsonProperty]
		public string IntermediateLocations { get; set; } 
	}

	// todo - make this JsonResult into a shared definition with the draw app 
	[DataContract]
	public class JsonEntry
	{
		[JsonProperty]
		public string RaceCode { get ; set; } 
		[JsonProperty]
		public string Location { get ; set; } 
		[JsonProperty]
		public int StartNumber { get ; set; } 
		[JsonProperty]
		public DateTime TimeStamp { get ; set; }
		// todo - do we need milliseconds, won't this be in the timestamp? 
		[JsonProperty]
		public int Milliseconds { get ; set; } 
		[JsonProperty]
		public bool Sequence { get ; set; } 
		[JsonProperty]
		public string Notes { get ; set; } 
	}
}

