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
using Head.Common.Utils;
using System.Configuration;

namespace Head.Common.Generate.Validators
{

	public class CrewValidator : IValidation<IEnumerable<ICrew>>
	{
		#region IValidation implementation

		public bool Validate (IEnumerable<ICrew> crews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			logger.Info ("Boating contact information:");
			foreach (var grouping in crews.GroupBy(cr => cr.BoatingLocation)) 
			{
				logger.InfoFormat("{0}: {1} => {2} ", grouping.Key.Name, grouping.Count(), grouping.Select(gr => gr.BoatingLocationContact).Distinct().Delimited());
			}

			// TODO - group together boats with mailing contacts for each loation 
			// TODO - highlight the crews with a note that they're marshalling out of position 

			logger.InfoFormat ("Unpaid emails: {0}", crews.Where (cr =>  !cr.IsPaid).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ()); // cr.IsAccepted &&
			logger.InfoFormat ("Junior emails: {0}", crews.Where (cr => cr.IsAccepted && cr.IsJunior).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());
			logger.InfoFormat ("Submitting emails: {0}", crews.Where (cr => cr.IsAccepted).Select (cr => cr.SubmittingEmail).Distinct ().Delimited ());

			logger.Info ("Unoffered event contacts:");
			foreach (var crew in crews.Where(cr => !cr.EventCategory.Offered)) 
			{
				logger.InfoFormat ("{0} || {1} || {2}", crew.Name, crew.EventCategory.Name, crew.SubmittingEmail);
			}
				
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

					// TODO - this is only a Vets Head thing 
//					if (!crew.Categories.Any (cat => cat.EventType == EventType.MastersHandicapped))
//						sb.Append ("Crew is expected to qualify for a masters handicapped gender competition. ");
//
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
