using System;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using Head.Common.Interfaces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Internal.Categories
{

	public class GenderCategory : BaseCategory
	{
		readonly Gender _gender;

		public GenderCategory(Gender gender) : base(EventType.Gender)
		{
			_gender = gender;
		}

		public Gender Gender { get { return _gender; } }

		#region implemented abstract members of BaseCategory

		protected override bool IsIncluded (ICrew crew)
		{
			return !crew.IsTimeOnly && crew.Gender == _gender;
		}

		public override string Name { get { return _gender.ToString (); } } 

		#endregion
	}
	
}
