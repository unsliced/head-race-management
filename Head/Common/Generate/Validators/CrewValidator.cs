using System;
using System.Collections.Generic;
using Common.Logging;
using Head.Common.Domain;
using System.Linq;
using System.Text;
using Head.Common.Utils;

namespace Head.Common.Generate.Validators
{
    ///
    public class CrewValidator : IValidation<IEnumerable<ICrew>>
	{
        readonly IList<IAthlete> _athletes;

        #region IValidation implementation

        public CrewValidator(IList<IAthlete> athletes)
        {
            _athletes = athletes;
        }

		public bool Validate (IEnumerable<ICrew> crews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			logger.Info ("Boating contact information:");
			foreach (var grouping in crews.Where(cr => cr.BoatingLocation != null).GroupBy(cr => cr.BoatingLocation)) 
			{
				logger.InfoFormat("{0}: {1} => {2} ", grouping.Key.Name, grouping.Count(), grouping.Select(gr => gr.BoatingLocationContact).Distinct().Delimited());
			}

            int expectedCrewMembers =_athletes.Select(a => a.Seat).Max();
            foreach(var grouping in _athletes.GroupBy(cr => cr.CrewId))
            {
                if (grouping.Count() != 4 && grouping.Count() != 9) // HACK: < expectedCrewMembers)
                {
                    logger.InfoFormat("{0} missing a crew member (has {1}) ", grouping.Key, grouping.Count());
                } 
            }

            // TODO - group together boats with mailing contacts for each loation 
            // TODO - highlight the crews with a note that they're marshalling out of position 

            logger.InfoFormat("Placeholder emails: {0}", 
                crews
                    .Where(cr => !new List<int> { 4, 9 }.Contains(_athletes.Count(a => a.CrewId == cr.CrewId))) // HACK: in  < expectedCrewMembers)
                    .Select(cr => cr.SubmittingEmail)
                    .Distinct().Delimited());
            logger.InfoFormat("Unpaid emails: {0}", crews.Where(cr => !cr.IsPaid).Select(cr => cr.SubmittingEmail).Distinct().Delimited()); // cr.IsAccepted &&
            logger.InfoFormat ("Junior emails: {0}", crews.Where (cr => cr.IsAccepted && cr.IsJunior).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());
			logger.InfoFormat ("Submitting emails: {0}", crews.Where (cr => cr.IsAccepted).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());


            logger.Info("Aggregated emails");
            var aggregatedcrews = crews.Where(cr => cr.EventCategory.IsAggregated).GroupBy(cr => cr.EventCategory.Name).ToList();
            foreach(var agg in aggregatedcrews)
            {
                logger.InfoFormat("{0}: {1}", agg.Key, agg.ToList().Select(cr => cr.SubmittingEmail).Distinct().Delimited());
            }

            logger.Info("Unoffered event contacts:");
            foreach (var crew in crews.Where(cr => !cr.EventCategory.Offered))
            {
                logger.InfoFormat("{0} || {1} || {2}", crew.Name, crew.EventCategory, crew.SubmittingEmail);
            }
            logger.Info("No boating location contacts:");
            foreach (var crew in crews.Where(cr => cr.BoatingLocation == null))
            {
               logger.InfoFormat("{0} || {1} || {2}", crew.CrewId, crew.Name, crew.SubmittingEmail);
            }

            logger.Info("Non-empty notes");
            foreach (var crew in crews.Where(cr => !string.IsNullOrEmpty(cr.Notes)))
            {
                logger.InfoFormat("{0} || {1} || {2}", crew.CrewId, crew.Name, crew.Notes);
            }



            bool valid = true;
			StringBuilder scratches = new StringBuilder();
			IDictionary<int, bool> present = new Dictionary<int, bool>();
			for (int i = 1; i <= crews.Max(cr => cr.StartNumber); i++)
			{
				present.Add(i, false);
			}
			foreach (var crew in crews) 
			{
				StringBuilder sb = new StringBuilder ();
				var cats = crew.Categories.ToList ();
				if (crew.IsTimeOnly) 
				{
					if (cats.Count > 1 || cats [0].EventType != EventType.TimeOnly)
						sb.Append ("Time only crews cannot have any other categories. ");
				} 
				else 
				{
					if (!crew.Categories.Any (cat => cat.EventType == EventType.Overall))
						sb.Append ("Crew is expected to qualify for the overall competition. ");

					// TODO - this is only a Vets Head thing 
//					if (!crew.Categories.Any (cat => cat.EventType == EventType.MastersHandicapped))
//						sb.Append ("Crew is expected to qualify for a masters handicapped gender competition. ");
//
					if (crew.Categories.Count (cat => cat.EventType == EventType.Category) > 1)
						sb.Append ("Crew is expected to have a single event category. ");

				}			

				if(crew.IsScratched)
					scratches.AppendFormat("Crew {0} [{1}]{2}", crew.StartNumber, crew.CrewId, Environment.NewLine);
                

				if (sb.Length > 0) 
				{
					logger.ErrorFormat ("{0} {1}: {2}", sb.ToString (), crew.ToString (), cats.Select (cat => cat.Name).Aggregate ((h, t) => String.Format ("{0},{1}", h, t)));
					valid = false;
				}
				present[crew.StartNumber] = true;
			}

         

			if (scratches.Length > 0)
			{
				logger.Info("Scratch Report:");
				logger.Info(scratches.ToString());
				foreach (KeyValuePair<int, bool> p in present.Where(p => !p.Value))
				{
					logger.InfoFormat("{0} is not in the input file.", p.Key); 
				}
			}
			if (valid)
				logger.Info ("Crews' categories validated.");
			else
				logger.WarnFormat ("Errors detected, invalid set of crews.");

			return valid;

		}

        #endregion
    }
	
}
