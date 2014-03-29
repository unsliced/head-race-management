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
		public double A { get ; set; } 
        
        [JsonProperty]
		public double B { get ; set; } 
        
        [JsonProperty]
		public double C { get ; set; } 
        
        [JsonProperty]
		public double D { get ; set; } 
        
        [JsonProperty]
		public double E { get ; set; } 
        
        [JsonProperty]
		public double F { get ; set; } 
        
        [JsonProperty]
		public double G { get ; set; } 
        
        [JsonProperty]
		public double H { get ; set; } 
        
        [JsonProperty]
		public double I { get ; set; } 

        // TODO - could embed this directly into the Json 
		public IDictionary<string, double> Adjustments { get { return new Dictionary<string, double> { { "A", A}, {"B", B}, {"C", C}, {"D", D}, {"E", E}, {"F", F}, {"G", G}, {"H", H}, {"I", I} }; } }
    }

}

