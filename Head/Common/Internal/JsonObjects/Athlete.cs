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

        public int Age { get { return _competitor.Age; } } 
        public DateTime DateOfBirth { get { return _competitor.DateOfBirth; } }

		public IClub Club { get { return _club; } } 
		public ICrew Crew { get { return _crew; } } 
		public int CrewId { get { return _competitor.CrewId; } } 
		public string Licence { get { return _competitor.Licence; } } 
		public bool IsCox { get { return _competitor.IsCox; } } 
		public int Seat { get { return _competitor.Position; } }

		public void SetCrew(ICrew crew)
		{
			_crew = crew;
		}

		public void PickAClub(IEnumerable<IClub> clubs)
		{
			_club = clubs.First (cl => cl.Index == _competitor.ClubIndex);
		}

		public IClub RawClub { get { return new AthleteClub (_competitor.ClubIndex, _competitor.ClubName); } }
    }
}
