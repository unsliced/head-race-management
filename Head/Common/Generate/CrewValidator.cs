using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;

namespace Head.Common.Generate
{
	public class ClubValidator : IValidation<IEnumerable<IClub>>
	{
		#region IValidation implementation

		public bool Validate (IEnumerable<IClub> clubs)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();
			bool valid = true;

			foreach (var club in clubs) 
			{
				if (club.Index.StartsWith ("Z") && String.IsNullOrEmpty (club.Country)) 
				{
					logger.WarnFormat ("Need a country for club: {0} - {1}", club.Index, club.Name);
					valid = false;
				}
			}

			return valid;
		}

		#endregion


	}

	public class CrewValidator : IValidation<IEnumerable<ICrew>>
	{
		#region IValidation implementation

		public bool Validate (IEnumerable<ICrew> crews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();
			bool valid = true;
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

					if (!crew.Categories.Any (cat => cat.EventType == EventType.MastersHandicapped))
						sb.Append ("Crew is expected to qualify for a masters handicapped gender competition. ");

					if (crew.Categories.Count (cat => cat.EventType == EventType.Category) > 1)
						sb.Append ("Crew is expected to have a single event category. ");

				}

				if (crew.BoatingLocation == null)
					sb.Append ("Crew is expected to have a boating location.");

				if (sb.Length > 0) 
				{
					logger.ErrorFormat ("{0} {1}: {2}", sb.ToString (), crew.ToString (), cats.Select (cat => cat.Name).Aggregate ((h, t) => String.Format ("{0},{1}", h, t)));
					valid = false;
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
