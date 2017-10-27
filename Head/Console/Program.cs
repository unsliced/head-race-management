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

            foreach (Gender gender in (Gender[])Enum.GetValues(typeof(Gender)))
            {
                var openBands = categories.Where(cat => cat is EventCategory).Select(c => (EventCategory)c).Where(c => c.Gender == gender && c.UseForCRI).Select(c => c.EventId).ToList();

                IList<Func<List<int>>> del = new List<Func<List<int>>>
                    {
                        () => crews.Where(cr => cr.Gender == gender).Select(cr => cr.CRI(false)).ToList(),
                        () => crews.Where(crew => categories.Where(cat => cat is EventCategory).Select(c => (EventCategory)c).Where(c => c.Gender == gender && c.UseForCRI).Select(c => c.EventId).Contains(crew.EventCategory.EventId)).Select(cr => cr.CRI(false)).ToList()
                    };

                foreach (var fn in del)
                {
                    var crisToInclude = fn(); // crews.Where(crew => openBands.Contains(crew.EventCategory.EventId)).Select(cr => cr.CRI).ToList();
                    crisToInclude.Sort();
                    if (crisToInclude.Count > 0)
                    {
                        Logger.DebugFormat("{0} crews [#{2}] {1}", gender, crisToInclude.Select(c => c.ToString()).Aggregate((h, t) => String.Format("{0}, {1}", h, t)), crisToInclude.Count);

                        int tally = 0;
                        int lower = 0;
                        for (int i = 0; i < bandProportions.Count; i++)
                        {
                            tally += bandProportions[i];
                            int upper = i + 1 == bandProportions.Count ? crisToInclude.Count - 1 : 1 + (int)Math.Floor((decimal)crisToInclude.Count * tally / 100);
                            Logger.DebugFormat("Band {0} [{1}, {2}{4}. #{3} ", i + 1, crisToInclude[lower], crisToInclude[upper], (i+1 == bandProportions.Count ? 1 : 0) + upper - lower, i + 1 < bandProportions.Count ? ")" : "]"); // crisToInclude[mag-1], crisToInclude[mag]);
                            lower = upper;
                        }
                        foreach(var b in new List<bool> { true, false})
                            Logger.InfoFormat("Correlation between CRI{2} and points ({1}): {0}",
                                PearsonCorrelation(crews.Where(cr => cr.Gender == gender).Select(cr => (double)cr.CRI(b)).ToList(), crews.Where(cr => cr.Gender == gender).Select(cr => (double)cr.Points).ToList()),
                                gender,
                                b ? "Max" : "");
                    }
                }

            }
            foreach (var b in new List<bool> { true, false })
                Logger.InfoFormat("Correlation between CRI{1} and points (all): {0}",
                    PearsonCorrelation(crews.Select(cr => (double)cr.CRI(b)).ToList(), crews.Select(cr => (double)cr.Points).ToList()),
                    b ? "Max" : "");

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
        static double PearsonCorrelation(IList<double> Xs, IList<double> Ys)
        {
            double sumX = 0;
            double sumX2 = 0;
            double sumY = 0;
            double sumY2 = 0;
            double sumXY = 0;

            int n = Xs.Count < Ys.Count ? Xs.Count : Ys.Count;

            for (int i = 0; i < n; ++i)
            {
                double x = Xs[i];
                double y = Ys[i];

                sumX += x;
                sumX2 += x * x;
                sumY += y;
                sumY2 += y * y;
                sumXY += x * y;
            }

            double stdX = Math.Sqrt(sumX2 / n - sumX * sumX / n / n);
            double stdY = Math.Sqrt(sumY2 / n - sumY * sumY / n / n);
            double covariance = (sumXY / n - sumX * sumY / n / n);

            return covariance / stdX / stdY;
        }

    }
}
