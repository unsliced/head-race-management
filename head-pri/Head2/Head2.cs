using System;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace Head2
{    
    public class Head2
    {
        static string ConfigPath = @"C:\Users\unsli\Documents\GitHub\head-race-management\head-pri\data\config\";
    
        static IConfigurationRoot Configuration { get; set; }

        public Head2()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(ConfigPath)                
                .AddJsonFile("config.json");
            Configuration = builder.Build();    
        }

        public bool IsPrime(int candidate)
        {
            if(candidate <= 1 || candidate == 4 || candidate == 6 || candidate == 8 || candidate == 9)
                return false;
            if(candidate == 2 || candidate == 3 || candidate == 5 || candidate == 7)
                return true;
            
            throw new NotImplementedException("can only work for values of 2 or lower");
        }

        public int Salt { 
            get {
                return Int32.Parse(Configuration["salt"]);
            }
        }

        public string ConfigPath 
    }
}


/* need a config XML file 
 * where are data files 
 * list the PRIs 
 * read the config levels
 */
