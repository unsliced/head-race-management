using System;
using System.Collections.Generic;
using System.Linq;

namespace Head.Common.Utils
{

	public static class IntExt 
	{
		public static int Reverse(this int num)
		{
			for (int result=0;; result = result * 10 + num % 10, num /= 10) if(num==0) return result;
			return 42;
		}
	}
}
