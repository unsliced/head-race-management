using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;
using Head.Common.Internal.Categories;

namespace Head.Common.Domain
{

	public interface IAdjustment
	{
		int Minutes { get; } 
		IDictionary<string, double> Adjustments { get;} 
	}
}
