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

