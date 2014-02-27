using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System;

namespace Head.Common.Internal.Overrides 
{
    [DataContract]
    public class AthleteOverride : IEquatable<AthleteOverride>
    {
        [JsonProperty]
        public int CrewId { get; set; } 
        [JsonProperty]
        public int Position { get; set; } 
        [JsonProperty]
        public string Name { get; set; } 

        public bool Equals(AthleteOverride other)
        {
            if(ReferenceEquals(this, other))
            {
                return true;
            } 
            if(other == null)
            {
                return false;
            }
            return CrewId == other.CrewId && Position == other.Position && Name == other.Name;
        }
        
        public override bool Equals(object obj)
        {
            if(!(obj is AthleteOverride))
                return false;
            
            return Equals((AthleteOverride)obj);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int result = CrewId;
                result = (result * 397) ^ Position;
                result = (result * 397) ^ Name.GetHashCode();
                return result;
            }
        }
    }
}
