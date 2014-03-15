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

namespace Head.Common.Generate
{
	public class CategoryCreator : BaseRawCreator<ICategory, RawEvent, CategoryOverride>
	{
		#region implemented abstract members of BaseCreator

		protected override IList<ICategory> InternalCreate ()
		{

			// todo - need to think through adding in the other categories 
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
				// categories.Add (new GenderCategory (gender)); // Not adding a gender category for the Vets Head as this is covered in the masters handicapped category 
				categories.Add (new ForeignCategory (gender));
				categories.Add (new MastersGenderAdjustedCategory (gender));
				categories.Add (new MastersNoviceGenderAdjustedCategory (gender));

			}

			return categories;
		}			

		#endregion
	}

}
