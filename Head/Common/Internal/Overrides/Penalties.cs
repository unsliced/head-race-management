using System;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides 
{
	public class Penalty
	{
        [JsonProperty]
        public int Boat { get ; set; } 

        [JsonProperty]
        public int Seconds { get ; set; } 

        [JsonProperty]
        public bool Disqualified { get ; set; } 
    
        [JsonProperty]
        public string Citation { get ; set; } 
    }

}

