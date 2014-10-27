using System;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides 
{
    public class WeighedIn
    {
		// urgent - future update to handle the lightweights 
        [JsonProperty]
        public int Boat { get ; set; } 

        [JsonProperty]
        public bool MadeWeight { get ; set; } 
    }
}
