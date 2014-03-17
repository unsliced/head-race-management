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
			StartPositionGenerator.Generate (crews);




			bool valid = 
				new CrewValidator().Validate (crews) 
				&& new ClubValidator().Validate (clubs) 
				&& new AthleteValidator().Validate (athletes);

			if (!valid)
				return;
				
			Logger.Info ("Application stopped.");
		}
	}
}
