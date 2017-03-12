using System;
using System.Linq;
using Head.Common.BritishRowing;
using Head.Common.Domain;
using Head.Common.Internal.Overrides;
using System.Collections.Generic;

namespace Head.Common.Internal.JsonObjects
{
    public class Athlete : IAthlete
    {
        readonly RawCompetitor _competitor;
        readonly AthleteOverride _athleteOverride;
		ICrew _crew;
		IClub _club;

		public Athlete(RawCompetitor competitor, AthleteOverride athleteOverride)
        {
            _competitor = competitor;
            _athleteOverride = athleteOverride;
        }

        public string Name 
        { 
            get 
            { 
                return _athleteOverride != null 
                    ? _athleteOverride.Name 
                    : String.Format("{0} {1}", _competitor.Initial, _competitor.Surname); 
            } 
        }

		public string FullName { get { return _athleteOverride != null 
				? _athleteOverride.Name 
					:  _competitor.FullName ; } }

        public int? Age { get { return _competitor.Age; } } 
		public int Points(bool sculling) 
		{ 
			return _competitor.IsCox ? 0 : _competitor.Points(sculling);
		}

		public IClub Club { get { return _club; } } 
		public ICrew Crew { get { return _crew; } } 
		public int CrewId { get { return _competitor != null ? _competitor.CrewId : _athleteOverride.CrewId ; } } 
		public string Licence { get { return _competitor.Licence; } } 
		public bool IsCox { get { return _competitor.IsCox; } } 
		public int Seat { get { return _competitor != null ? _competitor.Position : _athleteOverride.Position; } }

		public bool HasRaw { get { return _competitor != null; } }

		public void SetCrew(ICrew crew)
		{
			_crew = crew;
		}

		public void PickAClub(IEnumerable<IClub> clubs)
		{
			_club = clubs.FirstOrDefault(cl => cl.Index == (_competitor != null ? _competitor.ClubIndex : _athleteOverride.Index));
		}

		public IClub RawClub { 
			get { 
				if (_competitor != null) 
					return new AthleteClub (_competitor.ClubIndex, _competitor.ClubName); 
				return null;
			}
		}
    }
}
