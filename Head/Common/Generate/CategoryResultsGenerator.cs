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
using Head.Common.Internal.JsonObjects;

namespace Head.Common.Generate
{

	public class CategoryResultsGenerator
	{
		public static void Generate (IEnumerable<ICategory> categories)
		{
			foreach (var category in categories) 
			{
				category.SetOrdering ();
			}
		}
	}
}