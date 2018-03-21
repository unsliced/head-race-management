using System.Collections.Generic;
using Head.Common.Domain;

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