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

	public class StartPositionGenerator
	{
		public static void Generate(IEnumerable<ICrew> ecrews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			IList<ICrew> crews = ecrews.ToList();
			if(crews.Any(cr => cr.StartNumber > 0))
			{
				logger.Info ("crews have start numbers. ");
				if(crews.Any(cr => cr.StartNumber <= 0))
					logger.Warn ("but some don't, that's not right - delete the start positions or fix thge JSON.");
				return;
			}
			foreach(var crew in 
				crews
					.OrderBy(cr => cr.Categories.First(cat => cat is EventCategory).Order)
					.ThenBy(cr => cr.PreviousYear.HasValue && cr.PreviousYear.Value <= 3 ? cr.PreviousYear.Value : 5)
				.ThenBy(cr => ((cr.CrewId % 100) * 1000000) + cr.CrewId))
			{
				logger.InfoFormat("{0}, {1}", crew.Name, crew.Categories.First(cat => cat is EventCategory).Name);
			}
		}
	}
}
