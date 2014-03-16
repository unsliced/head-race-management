using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;

namespace Head.Common.Generate
{

	public class AthleteClubMapper
	{
		public static void Map(IEnumerable<IAthlete> athletes, IEnumerable<IClub> clubs)
		{
			foreach (var athlete in athletes)
				athlete.PickAClub (clubs);
		}
	}
}
