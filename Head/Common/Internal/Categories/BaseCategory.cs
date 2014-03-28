using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{
	public abstract class BaseCategory : ICategory
	{
		readonly EventType _eventType;
		readonly IList<ICrew> _crews = new List<ICrew>();
		bool _offered;

		protected BaseCategory(EventType eventType)
		{
			_eventType = eventType;
			_offered = true;
		}

		protected abstract bool IsIncluded(ICrew crew);

		#region ICategory implementation

		public virtual int Order { get { return -1; } } 
		public EventType EventType { get { return _eventType; } }
		public abstract string Name { get; } 
		public void FilterCrews (IEnumerable<ICrew> crews)
		{
			foreach (var crew in crews.Where(cr => IsIncluded(cr))) 
			{
				_crews.Add (crew);
				crew.IncludeInCategory (this);
			}
		}

		public void SetNotOffered() 
		{
			_offered = false;
		}
		public IEnumerable<ICrew> Crews { get { return _crews; } }
		public virtual bool Offered { get { return _offered; } } 

		public virtual void SetOrdering ()
		{
			int counter = 0;
			foreach (var crew in Crews.Where(cr => cr.FinishType == FinishType.Finished).OrderBy(cr => cr.Elapsed)) 
			{
				crew.SetCategoryOrder (this, ++counter);
			}
		}

		#endregion

		public bool Equals(ICategory other)
		{
			if (ReferenceEquals(this, other))
				return true;
			if (other == null)
				return false;
			return this.EventType == other.EventType && this.Name == other.Name;

		}

		public override int GetHashCode()
		{
			int result = EventType.GetHashCode();
			result = (result * 397) ^ Name.GetHashCode();
			return result;
		}

		public override bool Equals(object obj)
		{
			if(obj is ICategory) return Equals((ICategory)obj);
			return false;
		}
	}
}
