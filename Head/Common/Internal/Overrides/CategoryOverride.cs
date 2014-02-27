using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Head.Common.Interfaces.Enums;

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
        public bool ShowMastersCategory { get; set; }

		[JsonProperty, JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get ; set; } 

        [JsonProperty]
        public bool ApplyHandicap { get; set; }

        [JsonProperty]
        public int Heavy { get ; set; } 

        [JsonProperty]
        public bool ShowDoB { get ; set; } 


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
			return EventId == other.EventId && Order == other.Order && other.Gender == Gender && Name == other.Name;
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
				result = (result * 397) ^ (int)Gender;
	            return result;
	        }
	    }
	}
}

