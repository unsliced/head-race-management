using System;
using System.Linq;
using Head.Common.BritishRowing;
using Head.Common.Domain;
using Head.Common.Internal.Overrides;

namespace Logic.Domain
{
    public class Athlete : IAthlete
    {
        readonly RawCompetitor _competitor;
        readonly AthleteOverride _athleteOverride;
		readonly IClub _club;

		public Athlete(RawCompetitor competitor, AthleteOverride athleteOverride, IClub club)
        {
            _competitor = competitor;
            _athleteOverride = athleteOverride;
			_club = club; // todo - or a list of clubs and confirm it from _competitor.ClubIndex 
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

		[Obsolete]
		public string ClubIndex { get { return _club.Index; } } 
		[Obsolete]
		public string ClubName { get { return _club.Name; } } 
    }
}
