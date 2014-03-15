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
	public class CategoryCrewMapper 
	{
		public static void Map(IEnumerable<ICategory> categories, IEnumerable<ICrew> crews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();
			foreach (var category in categories.ToList().OrderBy(c => c.Order).ThenBy(c => c.Name)) // ß, new CategoryOrderNameHelper()))
			{
				category.FilterCrews (crews);
				int counter = category.Crews.Count ();
				if (counter < 3)
					category.SetNotOffered();
				if(counter > 0)
					logger.Debug (d => d("Category: {0}. # crews: {1}, order: {2}. {3}", category.Name, counter, category.Order, category.Offered));
			}
		}
	}

	public class AthleteClubMapper
	{
		public static void Map(IEnumerable<IAthlete> athletes, IEnumerable<IClub> clubs)
		{
			foreach (var athlete in athletes)
				athlete.PickAClub (clubs);
		}
	}
}
