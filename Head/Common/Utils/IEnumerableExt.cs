using System;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Utils
{
	public static class IEnumerableExt
	{
		public static string Delimited(this IEnumerable<string> items, char delimiter = ',')
		{
			return items.Count () == 0 
				? String.Empty 
				: items.Aggregate ((h, t) => String.Format ("{0}{1}{2}", h, delimiter, t));
		}
	}
}

