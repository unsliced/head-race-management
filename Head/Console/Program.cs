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
			var originalathletes = new AthleteCreator().SetRawPath ("Resources/competitorexport-close.csv").Create ();
			var clubs = new ClubCreator(athletes).SetOverrideFactory("Resources/clubs.json").Create ();

			AthleteClubMapper.Map(athletes, clubs);

			var startpositions = new StartPositionFactory("Resources/startpositions.json").Create();
			var crews = new CrewCreator(categories, clubs, startpositions, athletes).SetRawPath("Resources/crewexport.csv").SetOverrideFactory("Resources/crews.json").Create ();		

			CategoryCrewMapper.Map(categories, crews);
			StartPositionGenerator.Generate (crews);

			Logger.Info ("Boating contact information:");
			foreach (var grouping in crews.GroupBy(cr => cr.BoatingLocation)) 
			{
				Logger.InfoFormat("{0}: {1} => {2} ", grouping.Key.Name, grouping.Count(), grouping.Select(gr => gr.BoatingLocationContact).Distinct().Delimited());
			}

			Logger.InfoFormat ("Unpaid emails: {0}", crews.Where (cr => !cr.IsPaid).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());

			Logger.Info ("Unoffered event contacts:");
			foreach (var crew in crews.Where(cr => !cr.EventCategory.Offered)) 
			{
				Logger.InfoFormat ("{0} || {1} || {2}", crew.Name, crew.EventCategory.Name, crew.SubmittingEmail);
			}

			Logger.Info ("Age report:");
			foreach(var athlete in athletes.Where(a => a.DateOfBirth >= new DateTime(2014,3,30).AddYears(-16) || a.DateOfBirth <= new DateTime(2014,3,30).AddYears(-75)).OrderBy(a => a.DateOfBirth))
				Logger.InfoFormat ("{0}, {1}, #{2} => {3}", athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber, athlete.DateOfBirth.ToShortDateString());
				
			Logger.Info ("Change report:");
			IList<Tuple<IAthlete, IAthlete>> changes = new List<Tuple<IAthlete, IAthlete>> ();
			foreach (var athlete in athletes) 
			{
				var originally = originalathletes.FirstOrDefault (a => a.Licence == athlete.Licence);
				if (originally == null) 
				{
					changes.Add (new Tuple<IAthlete, IAthlete> (athlete, null));
					Logger.InfoFormat ("{2}: {1}: {0} is new", athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber);
					continue;
				}
				if (athlete.CrewId != originally.CrewId) 
				{
					changes.Add (new Tuple<IAthlete, IAthlete> (athlete, originally));
					Logger.InfoFormat ("{2}: {1}: {0} has moved crew (from {3})", athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber, originally.CrewId);
				}
			}

			foreach (var grouping in changes.GroupBy(ch => ch.Item1.Crew.StartNumber).OrderBy(gr => gr.Key)) 
			{
				Logger.InfoFormat ("Crew {0} has made {1} changes: ", grouping.Key, grouping.Count ());
			}

			bool valid = 
				new CrewValidator().Validate (crews) 
				&& new ClubValidator().Validate (clubs);

			if (!valid)
				return;
				
			Logger.Info ("Application stopped.");
		}
	}
}
