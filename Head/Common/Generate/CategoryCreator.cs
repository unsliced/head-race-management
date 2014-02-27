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
					.Select (u => new EventCategory (u, null)));

			foreach (Gender gender in (Gender[]) Enum.GetValues(typeof(Gender)))
			{
				// categories.Add (new GenderCategory (gender)); // Not adding a gender category for the Vets Head as this is covered in the masters handicapped category 
				categories.Add (new ForeignCategory (gender));
				categories.Add (new MastersGenderAdjustedCategory (gender));
			}

			return categories;
		}			

		#endregion
	}

	public class CrewCreator : BaseRawCreator<ICrew, RawCrew, CrewOverride>
	{
		readonly IDictionary<int, EventCategory> _eventCategories; 
		readonly IDictionary<int, int> _startPositions; 

		public CrewCreator(IEnumerable<ICategory> eventCategories, IDictionary<int, int> startPositions)
		{
			_eventCategories = eventCategories.Where(cat => cat is EventCategory).Select( cat => (EventCategory)cat).ToDictionary (ec => ec.EventId, ec => ec);
			_startPositions = startPositions;
		}

		#region implemented abstract members of BaseCreator

		protected override IList<ICrew> InternalCreate ()
		{
			IList<ICrew> crews = new List<ICrew> ();
			foreach (var raw in RawUnderlying) 
			{
				EventCategory eventCategory = _eventCategories [raw.eventId];
				CrewOverride crewOverride = RawOverrides.FirstOrDefault (o => o.CrewId == raw.crewId);
				int startPosition = _startPositions == null ? -1 : _startPositions [raw.crewId];
				ICrew crew = new Crew (raw, eventCategory, crewOverride, startPosition);
				crews.Add (crew);
			}
			return crews;
		}

		#endregion
	}
}
