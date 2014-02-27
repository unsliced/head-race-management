using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{

	public class OverallCategory : BaseCategory
	{
		public OverallCategory() : base(EventType.Overall)
		{
		}

		protected override bool IsIncluded(ICrew crew)
		{
			return !crew.IsTimeOnly;
		}

		public override string Name { get { return "Overall"; } }
	}
	
}
