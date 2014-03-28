using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Head.Common.Domain;

namespace Head.Common.Internal.Overrides 
{
    [DataContract]
	public class StartPosition : IStartPosition 
	{
        [JsonProperty]
        public int CrewId { get ; set; } 
        [JsonProperty]
        public int StartNumber { get ; set; } 
   	}

}

