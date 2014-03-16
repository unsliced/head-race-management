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

	// TODO - need to consume the original entrant to measure the number of changes 
	
}
