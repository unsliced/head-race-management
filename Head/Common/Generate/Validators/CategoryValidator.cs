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
using System.Configuration;

namespace Head.Common.Generate.Validators
{
	public class CategoryValidator : IValidation<IEnumerable<ICategory>> 
	{
		#region IValidation implementation

		public bool Validate (IEnumerable<ICategory> categories)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();
			logger.Info ("category drill down:"); 
			int showAthlete = Int32.Parse(ConfigurationManager.AppSettings ["showcompetitor"].ToString ());
			foreach (var cat in categories.Where(c => c.EventType == EventType.Category).Select(c => (EventCategory)c).OrderBy(c => c.Order)) 
			{
				if (cat.ShowMastersCategory || cat.ShowPoints) 
				{
					foreach (var crew in cat.Crews) {
						logger.InfoFormat ("{0}: {1} {2}", crew.CategoryName, crew.AthleteName(showAthlete, true), crew.Name);
					}
				}
			}

			return true;
		}

		#endregion
	}
	
}
