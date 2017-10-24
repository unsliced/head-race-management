using Head.Common.Generate;
using Common.Logging;
using System.Linq;
using Head.Common.Generate.Validators;
using Head.Common.Csv;
using System.Configuration;

namespace Head.Console
{
    class MainClass
	{
        static ILog Logger = LogManager.GetCurrentClassLogger ();

		public static void Main (string[] args)
		{
			Logger.Info("Application Started");

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["csvpath"].ToString()))
                CsvImporter.CsvPath = ConfigurationManager.AppSettings["csvpath"].ToString();
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["jsonpath"].ToString()))
                JsonOverrideFactory.JsonPath = ConfigurationManager.AppSettings["jsonpath"].ToString();

            var categories = 
				new CategoryCreator()
					.SetRawPath ("Events.csv")
					.SetOverrideFactory("events.json")
					.Create();
			var athletes = 
                new AthleteCreator()
                    .SetRawPath ("Competitors.csv")
                    .SetOverrideFactory("athletes.json")
                    .Create ();

			var clubs = new ClubCreator(athletes).SetOverrideFactory("clubs.json").Create ();

			AthleteClubMapper.Map(athletes, clubs);

			var startpositions = new StartPositionFactory("startpositions.json").Create();
			var crews = 
                new CrewCreator (categories, clubs, startpositions, athletes)
                    .SetRawPath ("Crews.csv")
                    .SetOverrideFactory ("crews.json")
                    .Create ()
                    .OrderBy (cr => cr.StartNumber)
                    .ToList ();


			CategoryCrewMapper.Map(categories, crews);

			if(args.Count() == 0 || args[0].ToLowerInvariant() != "results")
			{ 
				StartPositionGenerator.Generate (crews);



				bool valid = 
					new CrewValidator(athletes).Validate (crews) 
					&& new ClubValidator().Validate (clubs) 
					&& new AthleteValidator().Validate (athletes)
					&& new CategoryValidator().Validate(categories);

				if (!valid)
					return;
			}
			else
			{
				var starttimes = new SequenceItemFactory("start-times.json").Create();
				var finishtimes = new SequenceItemFactory("finish-times.json").Create();
				var penalties = new PenaltyFactory("penalties.json").Create();
				var adjustments = new AdjustmentFactory ("adjustments.json").Create ();
				// todo - is there a weighed in file? 

				TimeMapper.Map (crews, starttimes, finishtimes);
				TimeMapper.Penalise (crews, penalties);
				TimeMapper.Adjust (crews, adjustments);

				CategoryResultsGenerator.Generate (categories);

				ResultsPrinter.Dump (crews);
			}

			Logger.Info ("Application stopped.");
		}
	}
}
