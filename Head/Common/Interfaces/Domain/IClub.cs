using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;
using Head.Common.Internal.Categories;

namespace Head.Common.Domain
{

	public interface IClub
	{
		string Name { get; }
		string Index { get; }
		string Country { get; } 
		bool IsBoatingLocation { get; }
		IEnumerable<ICrew> BoatingCrews { get; }
		void AddBoatingCrew (ICrew crew);
	}

}
