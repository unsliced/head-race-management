using System;
using System.Linq;
using System.Collections.Generic;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using Head.Common.Utils;
using Common.Logging;

namespace Head.Common.Internal.JsonObjects
{

	public class Crew : ICrew
	{
		static ILog Logger = LogManager.GetCurrentClassLogger ();

		readonly IDictionary<ICategory, int> _categories;
		readonly EventCategory _eventCategory;
		readonly RawCrew _rawCrew;
		readonly CrewOverride _crewOverride;
        readonly IList<IAthlete> _athletes; 
        readonly int _startNumber;
		IClub _boatingLocation;
		readonly IList<IClub> _clubs;
		DateTime _start = DateTime.MinValue;
		DateTime _finish = DateTime.MinValue;
		TimeSpan _elapsed;
		TimeSpan _adjusted;
		TimeSpan _adjustment = TimeSpan.Zero;
		TimeSpan _penalty;
		string _citation = string.Empty;
		bool _disqualified;
		string _queryReason = string.Empty;

		public Crew(RawCrew rawCrew, EventCategory eventCategory, CrewOverride crewOverride, IClub boatingLocation, int startNumber, IEnumerable<IClub> clubs)
		{
			_rawCrew = rawCrew;
			_eventCategory = eventCategory;
			_categories = new Dictionary<ICategory, int>();
			_crewOverride = crewOverride;
            _athletes = new List<IAthlete>();
            _startNumber = startNumber;
			_boatingLocation = boatingLocation;
			if (boatingLocation != null)
				boatingLocation.AddBoatingCrew (this);
			_clubs = clubs.ToList ();
		}

		#region ICrew implementation

		public bool IsTimeOnly { get { return _crewOverride != null && _crewOverride.TimeOnly; } } 
		public Gender Gender { get { return _eventCategory.Gender; } } 
		public EventCategory EventCategory { get { return _eventCategory; } }  
		// note that this does mean that any crew with an override has to explicitly be set to foreign or not 
		public bool IsForeign { get { return _crewOverride != null ? _crewOverride.IsForeign : _rawCrew.submittingClubIndex.StartsWith("Z");}} 
		public bool IsMasters { get { return _eventCategory.IsMasters; } } 
		public IEnumerable<ICategory> Categories { get { return _categories.Keys; }  } 
		public IClub BoatingLocation { get { return _boatingLocation; } } 
		public string BoatingLocationContact { get { return _rawCrew.boatingPermissionClubEmail; } }
		public bool IsNovice { get { return _eventCategory.IsNovice; } } 
		public void IncludeInCategory (ICategory category)
		{
			_categories.Add (category, 0);
		}
			
		public string Name { 
			get { 
				return (_crewOverride != null && !String.IsNullOrEmpty (_crewOverride.CrewName)) 
					? _crewOverride.CrewName 
					: _clubs.Select (cl => cl.Name).Delimited ('/');
			} 
		} 

		public string AthleteName (int seat)
		{
			IAthlete athlete = _athletes.Where (a => a.Seat == seat).FirstOrDefault ();
			return athlete == null ? "???" : athlete.Name;
		}


		public bool IsScratched { get { return _rawCrew.scratched; } } 
		public bool IsPaid { get { return _rawCrew.paid; } } 
		public string SubmittingEmail { get { return _rawCrew.submittingAdministratorEmail; } } 


		public void SetTimeStamps (IEnumerable<DateTime> starts, IEnumerable<DateTime> finishes)
		{
			var lStarts = starts.ToList ();
			var lFinishes = finishes.ToList ();
			if (lStarts.Count == 1 && lFinishes.Count == 1) 
			{
				_start = starts.First(); 
				_finish = finishes.First();
				_elapsed = _finish - _start;
				_adjusted = _elapsed;
				return;
			}
			if (lStarts.Count > 1) {
				_queryReason = _queryReason + "Multiple start times. ";				 
			}
			if (lFinishes.Count > 1) {
				_queryReason = _queryReason + "Multiple finish times. ";				 
			}
		}

		public string QueryReason { get { return _queryReason; } } 

		public void SetAdjusted (TimeSpan adjustment)
		{
			_adjustment = adjustment;
			_adjusted = _adjusted.Add (-adjustment);
		}
		public DateTime StartTime { get { return _start; } } 
		public DateTime FinishTime { get { return _finish; } } 
		public TimeSpan Elapsed { get { return _elapsed; } } 
		public TimeSpan Adjusted { get { return _adjusted; } } 
		public TimeSpan Adjustment { get { return _adjustment; } }

		public FinishType FinishType {
			get {
				if(!string.IsNullOrEmpty(_queryReason) || _elapsed.TotalMilliseconds < 0)
					return FinishType.Query;
				if (_disqualified)
					return FinishType.DSQ;
				if (_start == DateTime.MinValue)
					return FinishType.DNS;
				if (_finish == DateTime.MinValue)
					return FinishType.DNF;
				if (IsTimeOnly)
					return FinishType.TimeOnly;
				return FinishType.Finished;
			}
		}

		public void SetPenalty (TimeSpan penalty, string citation)
		{
			if (penalty.TotalMilliseconds < 0)
				Logger.WarnFormat ("Trying to penalise crew {0} by a negative amount.", StartNumber);
			_penalty = penalty;
			_citation = string.Format("{0} ({1} seconds) ",citation, penalty.ToString().Substring(6));
		}


		public void Disqualify(string citation)
		{
			_disqualified = true;
			_citation = citation;
		}

		public string Citation { get { return _citation; } }

		public void SetCategoryOrder (ICategory category, int order)
		{
			if (_categories.ContainsKey (category))
				_categories [category] = order;
			else
				Logger.WarnFormat ("cannot set crew {0} to have category position {1} - it's not competing for it", StartNumber, category.Name);
		}

		public int CategoryPosition (ICategory category)
		{
			return _categories.ContainsKey (category) ? _categories [category] : 0;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("{0}, {1}, {2}", 
				StartNumber.ToString().PadLeft(3), 
				_rawCrew.crewName, // String.Join(",", ClubIndices),
				CategoryName.PadRight(26));
		}

        int AverageAge
        {
            get 
            {
                if(_athletes.Count == 0) return 0;               
				decimal sum = _athletes.Select(a => a.Age).Sum();
				return (int)Math.Round(sum/_athletes.Count, MidpointRounding.AwayFromZero);
            }
        }

		int AveragePoints
		{
			get 
			{
				if(_athletes.Count == 0) return 0;
				decimal sum = _athletes.Select(a => a.Points(EventCategory.IsSculling)).Sum();
				return (int)Math.Round(sum/_athletes.Count, MidpointRounding.AwayFromZero);
			}
		}

        public string CategoryName 
        { 
            get 
            { 
				if(IsMasters && EventCategory.IsMasters && EventCategory.ShowMastersCategory)
					return string.Format("{0} ({1}:{2})", EventCategory.Name, AverageAge, AverageAge.ToMastersCategory());
				if(EventCategory.ShowPoints)
					return string.Format("{0} ({1}:{2})", EventCategory.Name, AveragePoints, AveragePoints.ToOpenCategory());

				return EventCategory.Name;
            } 
        } 

        public int CrewId { get { return _rawCrew.crewId; } }

		#region old ICrew implementation

        public IList<string> ClubIndices { 
            get { 
                return _athletes.Count == 0 
                    ? new List<string> {  _rawCrew.submittingClubIndex } 
					: _athletes.Select(a => a.Club.Index).Distinct().ToList();

            } 
        } 
        public void AddAthlete(IAthlete athlete) 
        { 
            _athletes.Add(athlete);
        }

        public IEnumerable<IAthlete> Athletes 
        {
            get 
            {
                return _athletes;
            }
        }

        public bool Paid { get { return _rawCrew.paid; } } 
       
        public bool TimeOnly { get { return _crewOverride != null && _crewOverride.TimeOnly; } } 
        public string Notes { get { return _rawCrew.clubNotes; } } 
        public string VoecNotes { get { return _crewOverride == null ? String.Empty : _crewOverride.Notes; } }
        public int? PreviousYear { get { return (_crewOverride != null && _crewOverride.PreviousYear > 0) ? _crewOverride.PreviousYear : (int?)null ; } } 

        public int StartNumber { get { return _startNumber; } } 

        bool _include = true;

        public void DoNotDraw() { _include = false; } 
        public bool Include { get { return !_rawCrew.rejected && !_rawCrew.withdrawn && !_rawCrew.scratched && _include ; } } 

        public string CollectionPoint { get { return _crewOverride.CollectionPoint; } } 

        public bool Heavy { get ; set; } 
                
        public string FullyQualifiedName { get; set;} 

       
        #endregion
	}

    public static class AgeExt
    {
        public static string ToMastersCategory(this int age)
        {
            if(age < 27)
                return String.Empty;
            if(age < 36)
                return "A";
            if(age < 43)
                return "B";
            if(age < 50)
                return "C";
            if(age < 55) 
                return "D";
            if(age < 60) 
                return "E";
            if(age < 65)
                return "F";
            if(age < 70)
                return "G";
            if(age < 75)
                return "H";
            return "I";
        }
    }

	public static class PointsExt
	{
		public static string ToOpenCategory(this int points)
		{
			if(points <= 0)
				return "Novice";
			if(points <= 2)
				return "IM3";
			if(points <= 4)
				return "IM2";
			if(points <= 6)
				return "IM1";
			if(points <= 9)
				return "SEN";
			return "Elite";
		}
	}
}

