using System;
using System.Linq;
using System.Collections.Generic;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;

namespace Head.Common.Internal.JsonObjects
{
	public class Crew : ICrew
	{
		readonly IList<ICategory> _categories;
		readonly EventCategory _eventCategory;
		readonly RawCrew _rawCrew;
		readonly CrewOverride _crewOverride;
        readonly IList<IAthlete> _athletes; 
        readonly int _startNumber;

		public Crew(RawCrew rawCrew, EventCategory eventCategory, CrewOverride crewOverride, int startNumber)
		{
			_rawCrew = rawCrew;
			_eventCategory = eventCategory;
			_categories = new List<ICategory>();
			_crewOverride = crewOverride;
            _athletes = new List<IAthlete>();
            _startNumber = startNumber;
		}

		#region ICrew implementation

		public bool IsTimeOnly { get { return _crewOverride != null && _crewOverride.TimeOnly; } } 
		public Gender Gender { get { return _eventCategory.Gender; } } 
		public EventCategory EventCategory { get { return _eventCategory; } }  
		public bool IsForeign { get { return _rawCrew.submittingClubIndex.StartsWith("Z") || (_crewOverride != null && _crewOverride.IsForeign); } } 
		public bool IsMasters { get { return _eventCategory.IsMasters; } } 
		public IEnumerable<ICategory> Categories { get { return _categories; }  } 

		public void IncludeInCategory (ICategory category)
		{
			_categories.Add (category);
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
        public string CategoryName 
        { 
            get 
            { 
				var category = Categories.First (c => c.EventType == EventType.Category) as EventCategory;
				return 
                String.Format("{0}{1}", 
						(Heavy ? category.Heavy : category).Name, 
						// todo - chris - was Category.ShowMastersCategory 
						false 
								? String.Format(" ({0}:{1})", AverageAge, AverageAge.ToCategory()) : String.Empty); 
            } 
        } 



        public int CrewId { get { return _rawCrew.crewId; } }

		#region old ICrew implementation

        // todo - need to show the crew name 
        public IList<string> ClubIndices { 
            get { 
                return _athletes.Count == 0 
                    ? new List<string> {  _rawCrew.submittingClubIndex } 
					: _athletes.Select(a => a.Club.Index).Distinct().ToList();

            } 
        } 
        public string Name { get { return (_crewOverride != null && !String.IsNullOrEmpty(_crewOverride.CrewName)) ? _crewOverride.CrewName : String.Empty; } } 
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
        public string BoatingCode { get { return _rawCrew.boatingPermissionClubIndexCode; } } 
		public IClub BoatingLocation 
		{ 
			get 
			{ 
				throw new NotImplementedException ();
				//				return _rawCrew.boatingPermissionClubName; 
			} 
		} 
        public bool TimeOnly { get { return _crewOverride != null && _crewOverride.TimeOnly; } } 
        public string Notes { get { return _rawCrew.clubNotes; } } 
        public string VoecNotes { get { return _crewOverride == null ? String.Empty : _crewOverride.Notes; } }
        public int? PreviousYear { get { return (_crewOverride != null && _crewOverride.PreviousYear > 0) ? _crewOverride.PreviousYear : (int?)null ; } } 

        public int StartNumber { get { return _startNumber; } } 
        public string SubmittingEmail { get { return _rawCrew.submittingAdministratorEmail; } } 

        bool _include = true;

        public void DoNotDraw() { _include = false; } 
        public bool Include { get { return !_rawCrew.rejected && !_rawCrew.withdrawn && !_rawCrew.scratched && _include ; } } 

        public string CollectionPoint { get { return _crewOverride.CollectionPoint; } } 

        public bool Heavy { get ; set; } 
        public string Birthdays
        {
            get
            {
                // todo - show all the birthdays
				return false // todo - was _category.ShowDoB 
					? _athletes [0].DateOfBirth.ToShortDateString() : String.Empty;
            }
        }
       

        

        public string FullyQualifiedName { get; set;} 

       
        #endregion
	}

    public static class AgeExt
    {
        public static string ToCategory(this int age)
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
}

