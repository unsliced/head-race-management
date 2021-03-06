using System;
using System.Collections.Generic;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Internal.JsonObjects;

namespace Head.Common.Generate
{
    public class CrewCreator : BaseRawCreator<ICrew, RawCrew, CrewOverride>
	{
		readonly IEnumerable<EventCategory> _eventCategories; 
		readonly IEnumerable<IStartPosition> _startPositions; 
		readonly IEnumerable<IClub> _clubs;
		readonly IEnumerable<IAthlete> _athletes;

		public CrewCreator(IEnumerable<ICategory> eventCategories, IEnumerable<IClub> clubs, IEnumerable<IStartPosition> startPositions, IEnumerable<IAthlete> athletes)
		{
			_eventCategories = eventCategories.Where(cat => cat is EventCategory).Select( cat => (EventCategory)cat);
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
				if (raw.currentCrewStatus != "Accepted" && raw.currentCrewStatus != "Submitted")
				{
                    Logger.DebugFormat("Crew {0} is {1}", raw.crewId, raw.currentCrewStatus); 
					continue;
				}
                CrewOverride crewOverride = RawOverrides.FirstOrDefault(o => o.CrewId == raw.crewId);


                // if there are several categories for the single event id, pick one with the appropriate CRI range. 
                // todo: deal with the situation where lightweights cannot be in the lowest or highest bands 
                var validcats = _eventCategories.Where(cat => cat.EventId == raw.eventId).ToList();
                if(crewOverride != null && !string.IsNullOrEmpty(crewOverride.EventName))
                {
                    var overridecats = _eventCategories.Where(cat => cat.Name == crewOverride.EventName).ToList();
                    if (overridecats.Count == 1)
                    {
                        validcats = overridecats;
                        Logger.InfoFormat("EventMove: from {0} to {1}: {2}", raw.eventOverrideName, crewOverride.EventName, raw.entriesSecretaryEmail);
                    }
                    else
                    {
                        // if the event name has been overridden, we need to map across to the agg Master 
                        var aggMasters = overridecats.Where(cat => cat.AggregationMaster != null).Select(cat => cat.AggregationMaster).ToList();
                        if (aggMasters.Count > 0)
                            validcats = new List<EventCategory> { aggMasters[0] };
                    }
                }

                // HACK: euch. The PRI Name hack to ensure that crews that should be novice but aren't, but then end in an aggregated category ... 
                EventCategory eventCategory = 
                    validcats.Count > 1 
                        ? validcats.FirstOrDefault(vc => vc.CriInRange(crewOverride != null && crewOverride.PRI > 0 ? crewOverride.PRI : raw.rowingPointsCri)) // todo - sculling vs rowing 
                        : validcats[0];

				int startPosition = -1;
				try
				{
					startPosition = _startPositions == null ? -1 : _startPositions.First(sp => sp.CrewId == raw.crewId).StartNumber;
				}
				catch (Exception)
				{
					Logger.ErrorFormat("A problem with the start position for crewid: {0}", raw.crewId);
					throw;
				}
				IClub boatingLocation = _clubs.FirstOrDefault (cl => !string.IsNullOrEmpty(raw.boatingPermissionClubIndexCode) && cl.Index == raw.boatingPermissionClubIndexCode);
				if (boatingLocation == null)
					Logger.WarnFormat ("Cannot identify boating location [crewid: {1}]: {0}", raw.boatingPermissionClubIndexCode, raw.crewId);
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
