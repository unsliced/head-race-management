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
using Head.Common.Internal.JsonObjects;
using System.Configuration;

namespace Head.Common.Generate
{
	public class CategoryCreator : BaseRawCreator<ICategory, RawEvent, CategoryOverride>
	{
		#region implemented abstract members of BaseCreator

		protected override IList<ICategory> InternalCreate ()
		{
			List<ICategory> categories = new List<ICategory>
				{
					new OverallCategory (),
					new TimeOnlyCategory ()
				};				

			categories.AddRange (
				RawUnderlying
				.Select (u => new EventCategory(u, RawOverrides.FirstOrDefault(ov => ov.EventId == u.eventId))));

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
					categories.Add (new MastersGenderAdjustedCategory (gender, true, true));
				}
			}

			return categories;
		}			

		#endregion
	}

}
