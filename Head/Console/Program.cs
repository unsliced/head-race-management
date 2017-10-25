using Head.Common.Generate;
using Common.Logging;
using System.Linq;
using Head.Common.Generate.Validators;
using Head.Common.Csv;
using System.Configuration;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System;

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

            // todo - this is almost certainly not the best place to do this 
            var openbands = ConfigurationManager.AppSettings["OpenBands"].Split(',').Select(i => Int32.Parse(i)).ToList();

            foreach (Gender gender in (Gender[])Enum.GetValues(typeof(Gender)))
            {
                var openBands = categories.Where(cat => cat is EventCategory).Select(c => (EventCategory)c).Where(c => c.Gender == gender && c.UseForCRI).Select(c => c.EventId).ToList();
                var crisToInclude = crews.Where(crew => openBands.Contains(crew.EventCategory.EventId)).Select(cr => cr.CRI).ToList();
                crisToInclude.Sort();
                if (crisToInclude.Count > 0)
                {
                    Logger.DebugFormat("{0} crews [#{2}] {1}", gender, crisToInclude.Select(c => c.ToString()).Aggregate((h, t) => String.Format("{0}, {1}", h, t)), crisToInclude.Count);
                
                    int tally = 0;
                    foreach (int b in openbands)
                    {
                        tally += b;
                        int mag = (int)Math.Floor((decimal)crisToInclude.Count * tally / 100);
                        Logger.DebugFormat("{0} band sum to item {1}, <= {2}", b, mag, crisToInclude[mag - 1]);
                    }
                }
            }
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
