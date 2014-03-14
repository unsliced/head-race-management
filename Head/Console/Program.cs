using System;
using Head.Common.Generate;
using Common.Logging;
using log4net.Config;
using System.Reflection;
using System.Linq;
using Head.Common.Domain;
using System.Text;

namespace Head.Console
{
	class MainClass
	{
		static ILog Logger = LogManager.GetCurrentClassLogger ();

		public static void Main (string[] args)
		{
			Logger.Info("Application Started");
		
			// TODO - sort the categories into order 
			var categories = 
				new CategoryCreator()
					.SetRawPath ("Resources/eventexport.csv")
					.SetOverrideFactory("Resources/events.json")
					.Create();
			var clubs = new ClubCreator().SetOverrideFactory("Resources/clubs.json").Create ();
			var crews = new CrewCreator(categories, clubs, null).SetRawPath("Resources/crewexport.csv").Create ();		

			CategoryCrewMapper.Map (categories, crews);

			// TODO - boating location report - including the email address(es) from the raw crew 
			Logger.Info ("Boating locations:");
			foreach (var location in clubs.Where(cl => cl.IsBoatingLocation))
				Logger.InfoFormat ("{0}: {1}", location.Name, location.BoatingCrews.Count());

			bool valid = new CrewValidator().Validate (crews);

			Logger.Info ("Application stopped.");
		}
	}
}
