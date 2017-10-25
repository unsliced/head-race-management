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
		readonly EventCategory _aggregationMaster;

		public EventCategory(RawEvent rawEvent, CategoryOverride categoryOverride, IList<EventCategory> masters) : base(EventType.Category)
		{
			if (categoryOverride == null)
				Logger.WarnFormat("unable to find JSON object for event ID: {0}", rawEvent.eventId);
			_rawEvent = rawEvent;
			_categoryOverride = categoryOverride;
			_aggregationMaster = null;
			if (_categoryOverride != null && masters != null)
				_aggregationMaster = masters.FirstOrDefault (m => m.Name == _categoryOverride.Name);
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
			return !crew.IsTimeOnly && (crew.EventCategory == this || crew.EventCategory.AggregationMaster == this);
		}

		public override void SetOrdering ()
		{
			if (_categoryOverride != null && _categoryOverride.AggregationMaster) {
				int counter = 0;
				foreach (var crew in Crews.Where(cr => cr.FinishType == FinishType.Finished).OrderBy(cr => cr.Adjusted)) {
					crew.SetCategoryOrder (this, ++counter);
				}
				return;
			}
			if (_aggregationMaster != null)
				return;
			
			base.SetOrdering ();
		}

		public override void FilterCrews (IEnumerable<ICrew> crews)
		{
			if (AggregationMaster != null)
				return;
			base.FilterCrews (crews);
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
		public bool IsMasters { get { return _rawEvent.eventType.Equals ("Master") || (_categoryOverride == null ? false : _categoryOverride.ShowMastersCategory); } } // hack: to override masters novices in an open BROE2 style
		public bool IsNovice { get { return _categoryOverride != null ? _categoryOverride.IsNovice : false ; } } 
		public bool IsJunior { get { return _rawEvent.eventType.Equals("Junior") ; } } 

		public bool IsSculling { get { return _rawEvent.scullingStatus; } } 
		public bool ShowMastersCategory { get { return _categoryOverride == null ? false : _categoryOverride.ShowMastersCategory; } } 
		public bool ShowPoints { get { return _categoryOverride == null ? false : _categoryOverride.ShowPoints; } }
		public bool ShowJuniorCategory { get { return _categoryOverride == null ? false : _categoryOverride.ShowJuniorCategory; } } 
		public string MastersCategory { get { return IsMasters ? _rawEvent.subCategory : string.Empty; } }

		public EventCategory AggregationMaster { get { return _aggregationMaster; } } 
		public Gender Gender { get { return _rawEvent.Gender; } }

        public bool CriInRange(int cri)
        {
            if (_categoryOverride == null)
                return true;
            if (_categoryOverride.FromPri == _categoryOverride.ToPri)
                return true;
            return (cri >= _categoryOverride.FromPri && cri < _categoryOverride.ToPri);
        }

        public bool UseForCRI
        {
            get
            {
                if (_categoryOverride == null)
                    return false;
                return _categoryOverride.UseCri;
            }
        }

	}
}

