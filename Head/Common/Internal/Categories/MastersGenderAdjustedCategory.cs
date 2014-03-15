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

		public MastersGenderAdjustedCategory(Gender gender) : base(EventType.MastersHandicapped)
		{
			_gender = gender;
		}

		public Gender Gender { get { return _gender; } }

		#region implemented abstract members of BaseCategory

		protected override bool IsIncluded (ICrew crew)
		{
			return crew.IsMasters && crew.Gender == _gender;
		}

		public override string Name { get { return String.Format ("Adjusted ({0})", _gender); } } 

		#endregion
	}

	public class MastersNoviceGenderAdjustedCategory : BaseCategory
	{
		readonly Gender _gender;

		public MastersNoviceGenderAdjustedCategory(Gender gender) : base(EventType.MastersHandicapped)
		{
			_gender = gender;
		}

		public Gender Gender { get { return _gender; } }

		#region implemented abstract members of BaseCategory

		protected override bool IsIncluded (ICrew crew)
		{
			return crew.IsMasters && crew.IsNovice && crew.Gender == _gender;
		}

		public override string Name { get { return String.Format ("Adjusted ({0} Novice)", _gender); } } 

		#endregion
	}


}
