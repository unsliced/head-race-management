using Head.Common.Generate;
using Common.Logging;
using System.Linq;
using Head.Common.Generate.Validators;
using Head.Common.Csv;
using System.Configuration;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System;
using System.Collections.Generic;

namespace Head.Console
{
    class MainClass
	{
        // URGENT - add the data and broe2 directories to gitignore and delete previous branches, although keep (most of the) config files 

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
            var bandProportions = ConfigurationManager.AppSettings["OpenBands"].Split(',').Select(i => Int32.Parse(i)).ToList();


            // todo: question - how to deal with a lightweight out of range? 
            foreach (Gender gender in (Gender[])Enum.GetValues(typeof(Gender)))
            {
                // HACK: differentiate between quads and eights for the cri 
                string catsize = "1x"; 
                // foreach (string catsize in new string[] { "8+", "4x" }) 
                {

                    IList<Func<List<int>>> del = new List<Func<List<int>>>
                    {
//                        () => crews.Where(cr => cr.Gender == gender).Select(cr => cr.CRI(false)).ToList(),
                        () => crews
                        .Where(crew => 
                            categories.Where(cat => cat is EventCategory).Select(c => (EventCategory)c).Where(c => c.Gender == gender && c.UseForCRI)
                          .Select(c => c.EventId).Contains(crew.EventCategory.EventId))
                        .Select(cr => cr.CRI(false)).ToList()
                    };

                    foreach (var fn in del)
                    {
                        var crisToInclude = fn(); // crews.Where(crew => openBands.Contains(crew.EventCategory.EventId)).Select(cr => cr.CRI).ToList();
                        crisToInclude.Sort();
                        if (crisToInclude.Count > 0)
                        {
                            Logger.DebugFormat("{0}/{3} crews [#{2}] {1}", gender, crisToInclude.Select(c => c.ToString()).Aggregate((h, t) => String.Format("{0}, {1}", h, t)), crisToInclude.Count, catsize);

                            int tally = 0;
                            int lower = 0;
                            for (int i = 0; i < bandProportions.Count; i++)
                            {
                                tally += bandProportions[i];
                                int upper = i + 1 == bandProportions.Count ? crisToInclude.Count - 1 : 1 + (int)Math.Floor((decimal)crisToInclude.Count * tally / 100);
                                Logger.DebugFormat("Band {0} [{1}, {2}{4}. #{3} ", i + 1, crisToInclude[lower], crisToInclude[upper], (i + 1 == bandProportions.Count ? 1 : 0) + upper - lower, i + 1 < bandProportions.Count ? ")" : "]"); // crisToInclude[mag-1], crisToInclude[mag]);
                                lower = upper;
                            }
                        }
                    }
                }

            }

            CategoryCrewMapper.Map(categories, crews, Int32.Parse(ConfigurationManager.AppSettings["NotOfferedLimit"]));

            switch(Properties.Settings.Default.DrawOrResults)
			{
                case "draw":
				    StartPositionGenerator.Generate (crews);

				    bool valid = 
					    new CrewValidator(athletes).Validate (crews) 
					    && new ClubValidator().Validate (clubs) 
					    && new AthleteValidator().Validate (athletes)
					    && new CategoryValidator().Validate(categories);

				    if (!valid)
					    return;
                    break;
                case "results":
                    var starttimes = new SequenceItemFactory("start-times.json").Create();
				    var finishtimes = new SequenceItemFactory("finish-times.json").Create();
				    var penalties = new PenaltyFactory("penalties.json").Create();
				    var adjustments = new AdjustmentFactory ("adjustments.json").Create ();
				    // todo - is there a weighed in file? 

				    TimeMapper.Map (crews, starttimes, finishtimes);
				    TimeMapper.Penalise (crews, penalties);
				    TimeMapper.Adjust (crews, adjustments);

				    CategoryResultsGenerator.Generate (categories);

				    ResultsPrinter.Dump (crews, ConfigurationManager.AppSettings["ResultsStatus"]);
                    break;
			}

			Logger.Info ("Application stopped.");
		}

    }
}
