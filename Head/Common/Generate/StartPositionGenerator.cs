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
using Head.Common.Utils;

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
				foreach (var crew in crews.OrderBy(cr => cr.StartNumber)) 
				{
					ICategory primary;
					string extras = String.Empty;
					if(crew.Categories.Any (c => c is TimeOnlyCategory)) 
					{ 
						primary = crew.Categories.First (c => c is TimeOnlyCategory);
					}
					else 
					{
						primary = crew.Categories.First (c => c is EventCategory);
						extras = crew.Categories.Where (c => !(c is EventCategory) && !(c is OverallCategory)).Select (c => c.Name).Delimited ();
					}

					logger.InfoFormat ("{0} {1} {2} {3}", crew.StartNumber, crew.Name, primary.Name, extras);
				}
				return;
			}
			IList<string> startpositions = new List<string> ();

			foreach(var crew in 
				crews
					.OrderBy(cr => cr.Categories.First(cat => cat is EventCategory).Order)
					.ThenBy(cr => cr.PreviousYear.HasValue && cr.PreviousYear.Value <= 3 ? cr.PreviousYear.Value : 5)
				.ThenBy(cr => ((cr.CrewId % 100) * 1000000) + cr.CrewId))
			{
				logger.InfoFormat("{0}, {1}", crew.Name, crew.Categories.First(cat => cat is EventCategory).Name);
				startpositions.Add(String.Format("{{\"CrewId\":{0},\"StartNumber\":{1}}}", crew.CrewId, startpositions.Count+1));
			}
			logger.Info(startpositions.Delimited());
		}
	}
}
