using System;
using Newtonsoft.Json;

namespace Head.Common.Internal.Overrides 
{
	public interface IPenalty
	{
		int StartNumber { get;}
		int Seconds { get; } 
		bool Disqualified { get;} 
		string Citation { get; } 
	}

	public class Penalty : IPenalty
	{
        [JsonProperty]
		public int StartNumber { get ; set; } 

        [JsonProperty]
        public int Seconds { get ; set; } 

        [JsonProperty]
        public bool Disqualified { get ; set; } 
    
        [JsonProperty]
        public string Citation { get ; set; } 
    }

}

