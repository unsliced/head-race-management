using System;
using System.Collections.Generic;
using Common.Logging;
using Head.Common.Domain;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Head.Common.Generate.Validators
{

    public class AthleteValidator : IValidation<IEnumerable<IAthlete>>
	{
		#region IValidation implementation
		public bool Validate (IEnumerable<IAthlete> athletes)
		{
            
            ILog logger = LogManager.GetCurrentClassLogger ();

			DateTime raceday = DateTime.MinValue;
			if(!DateTime.TryParse(ConfigurationManager.AppSettings["racedate"].ToString(), out raceday))
				raceday = DateTime.MinValue;

			var sb = new StringBuilder ();

			sb.AppendLine ("Scratches:");
			foreach (var athlete in athletes.Where(a => !a.HasRaw))
				sb.AppendFormat ("Crew {0} [{1}], {2}: {3}{4}", athlete.CrewId, athlete.Seat, athlete.Club.Index, athlete.Name, Environment.NewLine); 

			sb.AppendLine("Age report:");
            // todo: configure the report ages 
            foreach (var athlete in athletes.Where(a => a.HasRaw).Where(a => a.Age <= 16 || a.Age >= 75).OrderBy(a => a.Age))
                sb.AppendFormat("{0}, {1}, #{2}, {6}, {7} => {4} years {3}{5}",
                    athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber,
                    string.Empty,
                    athlete.Age, Environment.NewLine, athlete.Crew.BoatingLocation.Name, athlete.Crew.SubmittingEmail);
			logger.Info (sb.ToString ());

			logger.Info ("Change report:");
			IList<Tuple<IAthlete, IAthlete>> changes = new List<Tuple<IAthlete, IAthlete>> ();
            var ac = new AthleteCreator().SetRawPath("CompetitorsClose.csv");
            if (ac.RawPresent())
            {
                IList<IAthlete> originalathletes = ac.Create();

                foreach (var athlete in athletes.Where(a => a.HasRaw).OrderBy(a => a.Crew.StartNumber))
                {
                    // the substring here ensures that we're ignoring the expiry date, so we're not counting renewals. 

                    var originally = originalathletes.FirstOrDefault(a => a.Licence.Substring(7) == athlete.Licence.Substring(7));
                    if (originally == null)
                    {
                        if (!athlete.IsCox)
                            changes.Add(new Tuple<IAthlete, IAthlete>(athlete, null));
                        logger.InfoFormat("{2}: {1}: {0} is new [cox? {3}]. Crew {4}.", athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber, athlete.IsCox, athlete.CrewId);
                        continue;
                    }
                    if (athlete.CrewId != originally.CrewId)
                    {
                        if (!athlete.IsCox)
                            changes.Add(new Tuple<IAthlete, IAthlete>(athlete, originally));
                        logger.InfoFormat("{2}: {1}: {0} has moved crew (from {3}) [cox? {4}]", athlete.Name, athlete.Crew.Name, athlete.Crew.StartNumber, originally.CrewId, athlete.IsCox);
                    }
                }
            }

			foreach (var grouping in changes.GroupBy(ch => ch.Item1.Crew.StartNumber).OrderBy(gr => gr.Key)) {
				var crew = grouping.First().Item1.Crew;
				int ch = grouping.Count ();
				string msg = ch >= 4 ? string.Format ("{0} / {1}", crew.Name, crew.SubmittingEmail) : string.Empty;
				logger.InfoFormat ("Crew {0} has made {1} changes. {2}", grouping.Key, grouping.Count (), msg);
			}

			return true;
		}
		#endregion
	}
}
