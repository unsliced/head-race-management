using System;
using Head.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// packages = https://docs.microsoft.com/tr-tr/dotnet/articles/core/tutorials/managing-package-dependency-versions 

namespace Head.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // logging - https://msdn.microsoft.com/en-us/magazine/mt694089.aspx
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation(
                "This is a test of the emergency broadcast system.");
                
            HeadService hs = new HeadService();
            var result = hs.SampleFunction(94);
            Console.WriteLine("Hello World! {0}", result);

            // working with JSON - https://medium.com/@agriciuc/working-with-json-net-in-net-core-rc2-part-1-5f3a65f4e11#.v872slf1x 

            // Defines the sources of configuration information for the 
            // application.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("the-key", "the-value"),
                });

            // Create the configuration object that the application will
            // use to retrieve configuration information.
            var configuration = builder.Build();

            // Retrieve the configuration information.
            var configValue = configuration["the-key"];
            Console.WriteLine($"The value for 'the-key' is '{configValue}'");
            configValue = configuration["the-other-key"];
            Console.WriteLine($"The value for 'the-other-key' is '{configValue}'");

            var overallmastershandicapped = configuration["overallmastershandicapped"];
            Console.WriteLine($"The value for 'overallmastershandicapped' is '{overallmastershandicapped}'");

            string racedatestring = configuration["racedate"];
            DateTime dt;
            if(DateTime.TryParse(racedatestring, out dt))
            {
                Console.WriteLine($"The race is going to be on '{dt.ToString("dd-MMM")}'");
            }
            Console.ReadLine();
        }
    }
}
