using System;
using System.Collections.Generic;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Configuration;

namespace Head.Common.Generate
{
    public class CategoryCreator : BaseRawCreator<ICategory, RawEvent, CategoryOverride>
	{
		#region implemented abstract members of BaseCreator

		protected override IList<ICategory> InternalCreate ()
		{
            ILog logger = LogManager.GetCurrentClassLogger();
            
            List<ICategory> categories = new List<ICategory>
				{
					new OverallCategory (),
					new TimeOnlyCategory ()
				};				

            // todo: review how to combine categories together more effectively - setting the 
			var masterOverrides = RawOverrides.Where (o => o.AggregationMaster).Select(o => o.EventId);
			IList<EventCategory> masters = 
				RawUnderlying
				.Where (u => masterOverrides.Contains (u.eventId))
				.Select (u => new EventCategory (u, RawOverrides.FirstOrDefault (ov => ov.EventId == u.eventId), null))
                .ToList();
			categories.AddRange (masters);
            var multiBands = RawOverrides.GroupBy(o => o.EventId).Where(kvp => kvp.Count() > 1).Select(kvp => kvp.Key).ToList();

            categories.AddRange (
				RawUnderlying
				.Where(u => !masters.Select(m => m.EventId).Contains(u.eventId))
                .Where(u => !multiBands.Contains(u.eventId))
				.Select (u => new EventCategory(u, RawOverrides.FirstOrDefault(ov => ov.EventId == u.eventId), masters)));

            foreach(var multi in multiBands)
            {
                var bands = RawOverrides.Where(o => o.EventId == multi);
                var underlying = RawUnderlying.FirstOrDefault(u => u.eventId == multi);
                foreach(var b in bands)
                {
                    categories.Add(new EventCategory(underlying, b, masters));
                    logger.DebugFormat("Added {0} ({1}, {2})", underlying.eventIdentity, b.FromPri, b.ToPri);
                }
            }

			foreach (Gender gender in (Gender[]) Enum.GetValues(typeof(Gender)))
			{
				if(ConfigurationManager.AppSettings["hasoverallgendercategory"].ToString() == "1") 
					categories.Add (new GenderCategory (gender)); 
				if(ConfigurationManager.AppSettings["hasoverallforeigncategory"].ToString() == "1") 
					categories.Add (new ForeignCategory (gender));
				if (ConfigurationManager.AppSettings ["overallmastershandicapped"].ToString () == "1") 
				{
					categories.Add (new MastersGenderAdjustedCategory (gender, false, false));
					categories.Add (new MastersGenderAdjustedCategory (gender, true, false));
					//categories.Add (new MastersGenderAdjustedCategory (gender, true, true));
				}
			}

			return categories;
		}			

		#endregion
	}

}
