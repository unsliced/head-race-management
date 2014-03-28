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
	// the idea here is a boat for which we have a time, but not one for which we know anything else 
	public class UnidentifiedCrew : ICrew
	{
		int _startNumber;
		DateTime _start;
		DateTime _finish;

		public UnidentifiedCrew(int startnumber, DateTime start, DateTime finish)
		{
			_startNumber = startnumber;
			_start = start;
			_finish = finish;
		}

		#region ICrew implementation

		public void IncludeInCategory (ICategory category){  throw new NotImplementedException (); }
		public void SetTimeStamps (DateTime start, DateTime finish){  throw new NotImplementedException (); }
		public void SetAdjusted (TimeSpan adjustment) {  throw new NotImplementedException (); }
		public void SetPenalty (TimeSpan penalty, string citation) { throw new NotImplementedException (); }
		public void Disqualify (string citation) { throw new NotImplementedException (); }
		public bool IsTimeOnly { get { throw new NotImplementedException (); } }
		public Gender Gender { get { throw new NotImplementedException (); } }
		public EventCategory EventCategory { get { throw new NotImplementedException (); } }
		public bool IsForeign { get { throw new NotImplementedException (); } }
		public bool IsMasters { get { throw new NotImplementedException (); } }
		public IEnumerable<ICategory> Categories { get { throw new NotImplementedException (); } }
		public string Name { get { return "Mystery Crew"; } }
		public IClub BoatingLocation { get { throw new NotImplementedException (); } }
		public int CrewId { get { throw new NotImplementedException (); } }
		public int StartNumber { get { return _startNumber; } } 
		public int? PreviousYear { get { throw new NotImplementedException (); } }
		public string BoatingLocationContact { get { throw new NotImplementedException (); } }
		public bool IsScratched { get { throw new NotImplementedException (); } }
		public bool IsPaid { get { throw new NotImplementedException (); } }
		public bool IsNovice { get { throw new NotImplementedException (); } }
		public IEnumerable<IAthlete> Athletes { get { throw new NotImplementedException (); } }
		public string SubmittingEmail { get { throw new NotImplementedException (); } }
		public TimeSpan Elapsed { get { return _finish - _start; } }
		public TimeSpan Adjusted { get { throw new NotImplementedException (); } }
		public FinishType FinishType { get { return FinishType.Query; } }
		public void SetCategoryOrder (ICategory category, int order) { throw new NotImplementedException (); } 
		public int CategoryPosition (ICategory category) { throw new NotImplementedException (); } 

		#endregion
	}
}
