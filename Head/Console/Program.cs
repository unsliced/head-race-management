using System;
using Head.Common.Generate;
using Common.Logging;
using log4net.Config;
using System.Reflection;
using System.Linq;
using Head.Common.Domain;
using System.Text;
using Head.Common.Utils;
using System.Collections.Generic;

namespace Head.Console
{
	class MainClass
	{
		static ILog Logger = LogManager.GetCurrentClassLogger ();

		public static void Main (string[] args)
		{
			Logger.Info("Application Started");
		
			var categories = 
				new CategoryCreator()
					.SetRawPath ("Resources/eventexport.csv")
					.SetOverrideFactory("Resources/events.json")
					.Create();
			var athletes = new AthleteCreator().SetRawPath ("Resources/competitorexport.csv").Create ();
			var clubs = new ClubCreator(athletes).SetOverrideFactory("Resources/clubs.json").Create ();

			AthleteClubMapper.Map(athletes, clubs);

			var startpositions = new StartPositionFactory("Resources/startpositions.json").Create();
			var crews = new CrewCreator(categories, clubs, startpositions, athletes).SetRawPath("Resources/crewexport.csv").SetOverrideFactory("Resources/crews.json").Create ();		

			CategoryCrewMapper.Map(categories, crews);

			if(args.Count() == 0 || args[0].ToLowerInvariant() != "results")
			{ 
				StartPositionGenerator.Generate (crews);


				bool valid = 
					new CrewValidator().Validate (crews) 
					&& new ClubValidator().Validate (clubs) 
					&& new AthleteValidator().Validate (athletes);

				if (!valid)
					return;
			}
			else
			{
				var starttimes = new SequenceItemFactory("Resources/start-times.json").Create();
				var finishtimes = new SequenceItemFactory("Resources/finish-times.json").Create();
				var penalties = new PenaltyFactory("Resources/penalties.json").Create();
				var adjustments = new AdjustmentFactory ("Resources/adjustments.json").Create ();
				// todo - is there a weighed in file? 

				TimeMapper.Map (crews, starttimes, finishtimes);
				TimeMapper.Penalise (crews, penalties);
				// TODO - calculate the adjustments based on fastest times and the CategoryAdjustment file 
				TimeMapper.Adjust (crews, adjustments);

				CategoryResultsGenerator.Generate (categories);

				ResultsPrinter.Dump (crews);
			}

			Logger.Info ("Application stopped.");
		}
	}
}
