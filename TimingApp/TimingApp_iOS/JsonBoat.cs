using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TimingApp_iOS
{
	// todo - make this into a shared definition with the draw app 
	[DataContract]
	public class JsonBoat
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
}

