using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{
	public class ForeignCategory : BaseCategory 
	{
		readonly Gender _gender;

		public ForeignCategory(Gender gender) : base(EventType.Foreign)
		{
			_gender = gender;
		}

		public Gender Gender { get { return _gender; } }

		#region implemented abstract members of BaseCategory

		protected override bool IsIncluded (ICrew crew)
		{
			// todo - the IsSculling is a VH hack to mitigate for octoples not being valid 
			return !crew.IsTimeOnly && crew.Gender == _gender && crew.IsForeign && !crew.EventCategory.IsSculling;
		}

		public override string Name { get { return string.Format("{0} Foreign (Adjusted)", _gender.ToString()); } } 

		#endregion

		public override void SetOrdering ()
		{
			int counter = 0;
			foreach (var crew in Crews.Where(cr => cr.FinishType == FinishType.Finished).OrderBy(cr => cr.Adjusted)) 
			{
				crew.SetCategoryOrder (this, ++counter);
			}
		}
	}	
}
