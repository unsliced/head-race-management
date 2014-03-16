using System;
using Head.Common.Generate;
using Common.Logging;
using log4net.Config;
using System.Reflection;
using System.Linq;
using Head.Common.Domain;
using System.Text;
using Head.Common.Utils;

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
			var crews = new CrewCreator(categories, clubs, startpositions, athletes).SetRawPath("Resources/crewexport.csv").Create ();		

			CategoryCrewMapper.Map(categories, crews);
			StartPositionGenerator.Generate (crews);

			Logger.Info ("Boating contact information:");
			foreach (var grouping in crews.GroupBy(cr => cr.BoatingLocation)) 
			{
				Logger.InfoFormat("{0}: {1} => {2} ", grouping.Key.Name, grouping.Count(), grouping.Select(gr => gr.BoatingLocationContact).Distinct().Delimited());
			}
			// TODO - generate a list of emails of the unpaid crews 
			Logger.InfoFormat ("Unpaid emails: {0}", crews.Where (cr => !cr.IsPaid).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());

			// TODO - generate the welfare report - under 16s, over 70s. 

			bool valid = 
				new CrewValidator().Validate (crews) 
				&& new ClubValidator().Validate (clubs);

			if (!valid)
				return;
				
			Logger.Info ("Application stopped.");
		}
	}
}
