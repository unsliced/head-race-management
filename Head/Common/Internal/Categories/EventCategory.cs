using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;

namespace Head.Common.Internal.Categories
{
	public class EventCategory : BaseCategory, IEquatable<EventCategory> 
	{
		static readonly ILog Logger = LogManager.GetCurrentClassLogger ();
		readonly RawEvent _rawEvent;
		readonly CategoryOverride _categoryOverride;

		public EventCategory(RawEvent rawEvent, CategoryOverride categoryOverride) : base(EventType.Category)
		{
			if (categoryOverride == null)
				Logger.WarnFormat("unable to find JSON object for event ID: {0}", rawEvent.eventId);
			_rawEvent = rawEvent;
			_categoryOverride = categoryOverride;
			// Heavy = this;
		}

		#region ICategory implementation

		public override string Name 
		{ 
			get 
			{ 
				return _categoryOverride == null || String.IsNullOrEmpty (_categoryOverride.Name) 
					? _rawEvent.eventIdentity.Replace (".1x", "").Replace (".8+", "")  
					: _categoryOverride.Name; 
			} 
		}

		protected override bool IsIncluded (ICrew crew)
		{
			return !crew.IsTimeOnly && crew.EventCategory == this;
		}

		#endregion

		#region IEquatable implementation

		public bool Equals (EventCategory other)
		{
			return EventId == other.EventId;
		}

		#endregion

		public int EventId { get { return _rawEvent.eventId; } } 
		public override int Order { get {  return (_categoryOverride != null && _categoryOverride.Order > 0) ? _categoryOverride.Order : _rawEvent.eventId; } } 
		public bool IsMasters { get { return _rawEvent.eventType.Equals ("Master"); } }
		public bool IsNovice { get { return _rawEvent.crewStatus == "Novice" ; } } 

		public bool IsSculling { get { return _rawEvent.scullingStatus; } } 
		public bool ShowMastersCategory { get { return _categoryOverride == null ? false : _categoryOverride.ShowMastersCategory; } } 
		public bool ShowPoints { get { return _categoryOverride == null ? false : _categoryOverride.ShowPoints; } }

		public string MastersCategory { get { return IsMasters ? _rawEvent.subCategory : string.Empty; } }

//		public string Name { 
//            get { 
//                return String.Format("{0}{1}", 
//                                     Name,                                      
//                                     ShowMastersCategory
//                                        ? String.Format(" ({0})", _rawEvent.subCategory)
//                                        : String.Empty); } }
//        public bool ShowDoB { get { return _categoryOverride.ShowDoB; } }  
		// urgent - need to implement heavy
//		public ICategory Heavy { get; set; } 
//        public int HeavyId { get { return _categoryOverride != null ? _categoryOverride.Heavy : 0 ; } }
		public Gender Gender { get { return _rawEvent.Gender; } }
//        public bool ApplyHandicap { get { return _categoryOverride == null ? false : _categoryOverride.ApplyHandicap; } } 
//        public string MastersCategory { get { return _rawEvent.subCategory;} } 

	}
}

