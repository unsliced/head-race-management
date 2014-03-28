using Newtonsoft.Json;
using System.Collections.Generic;
using Head.Common.Domain;

namespace Head.Common.Internal.Overrides 
{
	public class CategoryAdjustment : IAdjustment 
    {
        [JsonProperty]
        public int Minutes { get ; set; } 
        
        [JsonProperty]
        public int A { get ; set; } 
        
        [JsonProperty]
        public int B { get ; set; } 
        
        [JsonProperty]
        public int C { get ; set; } 
        
        [JsonProperty]
        public int D { get ; set; } 
        
        [JsonProperty]
        public int E { get ; set; } 
        
        [JsonProperty]
        public int F { get ; set; } 
        
        [JsonProperty]
        public int G { get ; set; } 
        
        [JsonProperty]
        public int H { get ; set; } 
        
        [JsonProperty]
        public int I { get ; set; } 

        // TODO - could embed this directly into the Json 
        public IDictionary<string, int> Adjustments { get { return new Dictionary<string, int> { { "A", A}, {"B", B}, {"C", C}, {"D", D}, {"E", E}, {"F", F}, {"G", G}, {"H", H}, {"I", I} }; } }
    }

}

