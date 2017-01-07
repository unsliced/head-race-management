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
	public class CrewCreator : BaseRawCreator<ICrew, RawCrew, CrewOverride>
	{
		readonly IDictionary<int, EventCategory> _eventCategories; 
		readonly IEnumerable<IStartPosition> _startPositions; 
		readonly IEnumerable<IClub> _clubs;
		readonly IEnumerable<IAthlete> _athletes;

		public CrewCreator(IEnumerable<ICategory> eventCategories, IEnumerable<IClub> clubs, IEnumerable<IStartPosition> startPositions, IEnumerable<IAthlete> athletes)
		{
			_eventCategories = eventCategories.Where(cat => cat is EventCategory).Select( cat => (EventCategory)cat).ToDictionary (ec => ec.EventId, ec => ec);
			_startPositions = startPositions;
			_clubs = clubs;
			_athletes = athletes;
		}

		#region implemented abstract members of BaseCreator

		protected override IList<ICrew> InternalCreate ()
		{
			IList<ICrew> crews = new List<ICrew> ();
			foreach (var raw in RawUnderlying) 
			{
				if (raw.withdrawn || raw.rejected ||  (raw.accepted && raw.scratched)) // !raw.accepted ||
				{
					Logger.DebugFormat ("Crew {0} is {1}{2}{3}", raw.crewId, 
						raw.withdrawn ? "withdrawn " : string.Empty, 
						raw.rejected ? "rejected " : string.Empty, 
						raw.scratched ? "scratched " : string.Empty, 
						!raw.accepted ? "not accepted " : string.Empty);
					continue;
				}
				EventCategory eventCategory = _eventCategories [raw.eventId];
				CrewOverride crewOverride = RawOverrides.FirstOrDefault (o => o.CrewId == raw.crewId);
				int startPosition = -1;
				try
				{
					startPosition = _startPositions == null ? -1 : _startPositions.First(sp => sp.CrewId == raw.crewId).StartNumber;
				}
				catch (Exception)
				{
					Logger.ErrorFormat("A problem with crewid: {0}", raw.crewId);
					throw;
				}
				IClub boatingLocation = _clubs.FirstOrDefault (cl => cl.Index == raw.boatingPermissionClubIndexCode);
				if (boatingLocation == null)
					Logger.WarnFormat ("Cannot identify boating location: {0}", raw.boatingPermissionClubIndexCode);
				var athletes = _athletes.Where (a => a.CrewId == raw.crewId).ToList();
				var componentClubs = athletes.Select(a => a.Club).Distinct().ToList();
				ICrew crew = new Crew (raw, eventCategory, crewOverride, boatingLocation, startPosition, componentClubs);
				foreach (var athlete in athletes) {
					athlete.SetCrew (crew);
					crew.AddAthlete (athlete);
				}
				crews.Add (crew);
			}
			return crews;
		}

		#endregion
	}
}
