using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides 
{
	[DataContract]
	public class CrewOverride : IEquatable<CrewOverride>
	{
		[JsonProperty]
		public int CrewId { get ; set; } 
		[JsonProperty]
		public string ClubIdentifier { get ; set; } 
		[JsonProperty]
		public string CrewName { get ; set; } 
		[JsonProperty]
		public string Country { get ; set; } 
		[JsonProperty]
		public int PreviousYear { get ; set; } 
		[JsonProperty]
		public string CollectionPoint { get ; set; } 
        [JsonProperty]
        public int EventId { get ; set; } 
        [JsonProperty]
        public string Notes { get ; set; } 
        [JsonProperty]
        public bool TimeOnly { get ; set; } 
        [JsonProperty]
        public bool IsForeign { get; set; }

		public bool Equals(CrewOverride other)
		{
			if(ReferenceEquals(this, other))
			{
				return true;
			} 
			if(other == null)
			{
				return false;
			}
			return CrewId == other.CrewId 
				&& ClubIdentifier == other.ClubIdentifier 
					&& other.CrewName == CrewName 
					&& other.CrewId == CrewId 
					&& Country == other.Country
                    && PreviousYear == other.PreviousYear
					&& CollectionPoint == other.CollectionPoint;
		}
		
		public override bool Equals(object obj)
		{
			if(!(obj is CrewOverride))
				return false;
			
			return Equals((CrewOverride)obj);
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				int result = CrewId;
				result = (result * 397) ^ ClubIdentifier.GetHashCode();
				result = (result * 397) ^ CrewName.GetHashCode();
				result = (result * 397) ^ CrewName.GetHashCode();
				result = (result * 397) ^ Country.GetHashCode();
				result = (result * 397) ^ Country.GetHashCode();
				result = (result * 397) ^ PreviousYear;
				return result;
			}
		}
	}
}

