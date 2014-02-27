using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Head.Common.Internal.Overrides 
{
    [DataContract]
    public class ClubDetails
	{
        [JsonProperty]
        public string Index { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Country { get; set; }
        [JsonProperty]
		public bool IsBoatingLocation { get; set; }

       
    }
}

