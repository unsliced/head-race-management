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
		public bool IsJunior { get { return _eventCategory.IsJunior; } } 

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

		public string ShortName 
		{
			get {
				string countries = _clubs.Where (cl => !string.IsNullOrEmpty (cl.Country)).Select (cl => cl.Country).Distinct ().Delimited ('/');
				return string.Format("{0}{1}{2}", 
					_rawCrew.submittingClub,
					(_clubs.Count > 1 ? " Composite" : string.Empty),
					string.IsNullOrEmpty(countries) ? string.Empty : " (" + countries + ")"
				); 
			}
		}

		public string AthleteName (int seat, bool full)
		{
			IAthlete athlete = _athletes.Where (a => a.Seat == seat).FirstOrDefault ();
			if (athlete != null)
				return full ? athlete.FullName : athlete.Name; 
			return "???";
		}


		public bool IsScratched { get { return _rawCrew.currentCrewStatus == "Scratched" || (_crewOverride != null && _crewOverride.IsScratched); } } 
		public bool IsAccepted { get { return _rawCrew.currentCrewStatus == "Accepted"; } } 
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
            _elapsed = _elapsed.Add(_penalty);
			_citation = string.Format("{0} ({1} seconds) ",citation, penalty.TotalSeconds);
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

		public string RawName { get { return _rawCrew.crewName; } }

        int? AverageAge
        {
            get 
            {
                if(_athletes.Count == 0) return 0;
                if (_athletes.Where(a => !a.Age.HasValue).Count() > 1) return null;          
				decimal sum = _athletes.Where(a => a.Seat != 0 && a.Age.HasValue).Select(a => a.Age.Value).Sum();
				return (int)Math.Floor(sum/_athletes.Where(a => a.Seat != 0).Count());
            }
        }

		public int Points
		{
			get 
			{
				if(_athletes.Count == 0) return 0;
                // todo - can use the rowing points total here 
				decimal sum = _athletes.Where(a => a.Seat != 0).Select(a => a.Points(EventCategory.IsSculling)).Sum();
				return (int)Math.Round(sum/_athletes.Where(a => a.Seat != 0).Count(), MidpointRounding.AwayFromZero);
			}
		}

        int CrewCri
        {
            get
            {
                if (_athletes.Count == 0) return 0;
                return EventCategory.IsSculling ? _rawCrew.scullingPointsCri : _rawCrew.rowingPointsCri;
            }
        }

        int CrewCriMax
        {
            get
            {
                if (_athletes.Count == 0) return 0;
                return EventCategory.IsSculling ? _rawCrew.scullingPointsCriMax : _rawCrew.rowingPointsCriMax;
            }
        }

        public string CategoryName 
        { 
            get 
            { 
				if (TimeOnly)
					return "Time Only";
				if (EventCategory.ShowJuniorCategory)
					// todo - holy magic number hack batman 
					return string.Format ("{0} ({1})", EventCategory.Name, _athletes.Count == 0 ? "n/a" : _athletes [0].Age >= 18 ? "J18" : "J17");
				if(IsMasters && EventCategory.IsMasters && EventCategory.ShowMastersCategory)
                    // todo - this should not fail for Masters category rowers 
					return string.Format("{0} ({1}{3}{2})", 
                        EventCategory.Name, 
                        (_crewOverride == null || string.IsNullOrEmpty(_crewOverride.MastersCategory)) ? AverageAge.Value.ToMastersCategory() : _crewOverride.MastersCategory, 
                        EventCategory.ShowPoints ? " " + Points.ToString() : "",
                        EventCategory.ShowPoints ? "/" + CrewCri : "");
				if(EventCategory.ShowPoints)
					return string.Format("{0} ({3}, {1}:{2})", EventCategory.Name, Points, Points.ToOpenCategory(), CrewCri);

				return EventCategory.Name;
            } 
        } 

        public int CrewId { get { return _rawCrew.crewId; } }
        public int CRI(bool max) {
            return max ? CrewCriMax : CrewCri;
        }
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
        public string Notes { get { return _rawCrew.notes; } } 
        public string VoecNotes { get { return _crewOverride == null ? String.Empty : _crewOverride.Notes; } }
        public int? PreviousYear { get { return (_crewOverride != null && _crewOverride.PreviousYear > 0) ? _crewOverride.PreviousYear : (int?)null ; } } 

        public int StartNumber { get { return _startNumber; } } 

        bool _include = true;

        public void DoNotDraw() { _include = false; } 
		public bool Include { get { return _rawCrew.currentCrewStatus != "Rejected" && _rawCrew.currentCrewStatus != "Withdrawn" && _include ; } } 

        public string CollectionPoint { get { return _crewOverride.CollectionPoint; } } 

        public bool Heavy { get ; set; } 
                
        public string FullyQualifiedName { get; set;}
        public string MastersCategory { get { return (_crewOverride == null || string.IsNullOrEmpty(_crewOverride.MastersCategory)) ? string.Empty : _crewOverride.MastersCategory; } }


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

