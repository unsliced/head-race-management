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
		
			var categories = new CategoryCreator ().SetRawPath ("Resources/eventexport.csv").Create ();
			var clubs = new ClubCreator().SetOverrideFactory("Resources/clubs.json").Create ();
			var crews = new CrewCreator(categories, null).SetRawPath("Resources/crewexport.csv").Create ();		

			CategoryCrewMapper.Map (categories, crews);
			CrewBoatingLocationMapper (crews, clubs);

			bool valid = new CrewValidator().Validate (crews);



			Logger.Info ("Application stopped.");
		}
	}
}
