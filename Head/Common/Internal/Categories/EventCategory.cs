using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{
	public class EventCategory : BaseCategory, IEquatable<EventCategory> 
	{
		readonly RawEvent _rawEvent;
		readonly CategoryOverride _categoryOverride;

		public EventCategory(RawEvent rawEvent, CategoryOverride categoryOverride) : base(EventType.Category)
		{
			_rawEvent = rawEvent;
			_categoryOverride = categoryOverride;
            Heavy = this;
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
			return crew.EventCategory == this;
		}

		#endregion

		#region IEquatable implementation

		public bool Equals (EventCategory other)
		{
			return EventId == other.EventId;
		}

		#endregion

		public bool IsMasters { get { return _rawEvent.eventType.Equals ("Master"); } } 

        public int EventId { get { return _rawEvent.eventId; } } 
//		public string Name { 
//            get { 
//                return String.Format("{0}{1}", 
//                                     Name,                                      
//                                     ShowMastersCategory
//                                        ? String.Format(" ({0})", _rawEvent.subCategory)
//                                        : String.Empty); } }
        public bool ShowMastersCategory { get { return _categoryOverride == null ? false : _categoryOverride.ShowMastersCategory; } } 
        public int Order { get {  return _categoryOverride.Order > 0 ? _categoryOverride.Order : _rawEvent.eventId; } } 
        public bool ShowDoB { get { return _categoryOverride.ShowDoB; } }  
		public ICategory Heavy { get; set; } 
        public int HeavyId { get { return _categoryOverride != null ? _categoryOverride.Heavy : 0 ; } }
		public Gender Gender { get { return _rawEvent.Gender; } }
        public bool ApplyHandicap { get { return _categoryOverride == null ? false : _categoryOverride.ApplyHandicap; } } 
        public string MastersCategory { get { return _rawEvent.subCategory;} } 

	}
}

