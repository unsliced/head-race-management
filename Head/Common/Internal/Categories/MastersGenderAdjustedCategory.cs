using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{
	public class MastersGenderAdjustedCategory : BaseCategory
	{
		readonly Gender _gender;
		readonly bool _isNovice;
		readonly bool _isSculling;

		public MastersGenderAdjustedCategory(Gender gender, bool isNovice, bool isSculling) : base(EventType.MastersHandicapped)
		{
			_gender = gender;
			_isSculling = isSculling;
			_isNovice = isNovice;
		}

		public Gender Gender { get { return _gender; } }

		#region implemented abstract members of BaseCategory

		protected override bool IsIncluded (ICrew crew)
		{
			return !crew.IsTimeOnly && crew.IsMasters && crew.IsNovice == _isNovice && crew.Gender == _gender && crew.EventCategory.IsSculling == _isSculling;
		}

		// chris - is sculling is a VH hack - could be alleviated with an override in the JSON 
		public override bool Offered { get { return !_isSculling && base.Offered; } } 
		public override string Name { get { return String.Format ("Adjusted ({0}{1})", _gender, _isNovice ? " Novice" : string.Empty); } } 

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
