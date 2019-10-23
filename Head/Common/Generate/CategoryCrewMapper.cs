using System.Collections.Generic;
using Common.Logging;
using Head.Common.Domain;
using System.Linq;

namespace Head.Common.Generate
{

    public class CategoryCrewMapper 
	{
		public static void Map(IEnumerable<ICategory> categories, IEnumerable<ICrew> crews, int notOfferedLimit)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();
			foreach (var category in categories.ToList().OrderBy(c => c.Order).ThenBy(c => c.Name)) // ß, new CategoryOrderNameHelper()))
			{
				category.FilterCrews (crews);
				int counter = category.Crews.Count ();
				if (counter < notOfferedLimit) 
					category.SetNotOffered();
				if(counter > 0)
					logger.Debug (d => d("Category: {0}. # crews: {1}, order: {2}. {3}", category.Name, counter, category.Order, category.Offered));
			}

            foreach(var category in categories.Where(cat => cat.EventType == EventType.Category).GroupBy(cat => cat.Name).ToList())
            {
                if (category.Count() > 1)
                {
                    logger.Debug(d => d("Aggregated category: {0}. #cats {1}", category.Key, category.Count()));
                    foreach(var cat in category.ToList())
                    {
                        cat.SetAggregated();
                    }
                }
            }
		}
	}
}