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

		protected BaseCategory(EventType eventType)
		{
			_eventType = eventType;
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

		public IEnumerable<ICrew> Crews { get { return _crews; } }

		#endregion
	}
}
