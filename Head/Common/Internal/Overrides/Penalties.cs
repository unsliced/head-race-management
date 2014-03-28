using System;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides 
{
	public class Penalty
	{
        [JsonProperty]
        public int Boat { get ; set; } 

		// TODO - handle the penalty seconds 
        [JsonProperty]
        public int Seconds { get ; set; } 

		// TODO - handle the disqualification  
        [JsonProperty]
        public bool Disqualified { get ; set; } 
    
		// TODO - handle the penalty citation 
        [JsonProperty]
        public string Citation { get ; set; } 
    }

}

