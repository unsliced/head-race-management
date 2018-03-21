using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides
{
    [DataContract]
	public class CategoryOverride : IEquatable<CategoryOverride>
	{
        [JsonProperty]
        public int EventId { get ; set; } 
        [JsonProperty]
        public int Order { get ; set; } 
		[JsonProperty]
		public string Name { get ; set; } 
		[JsonProperty]
		public bool ShowPoints { get ; set; } 
		[JsonProperty]
		public bool ShowMastersCategory { get ; set; } 
		[JsonProperty]
		public bool ShowJuniorCategory { get; set; } 
		[JsonProperty]
		public bool AggregationMaster { get; set; }
        [JsonProperty]
        public bool IsNovice { get; set; }

        [JsonProperty]
        public int FromPri { get; set; }
        [JsonProperty]
        public int ToPri { get; set; }
        [JsonProperty]
        public bool UseCri { get; set; }

        public bool Equals(CategoryOverride other)
		{
			if(ReferenceEquals(this, other))
			{
				return true;
			} 
			if(other == null)
			{
				return false;
			}
			return EventId == other.EventId && Order == other.Order && Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			if(!(obj is CategoryOverride))
				return false;
				
			return Equals((CategoryOverride)obj);
		}

		public override int GetHashCode()
	    {
	        unchecked
	        {
				int result = EventId;
				result = (result * 397) ^ Order;
	            result = (result * 397) ^ Name.GetHashCode();
	            return result;
	        }
	    }
	}
}
	