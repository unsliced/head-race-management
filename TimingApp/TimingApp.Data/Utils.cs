﻿using System;
using System.Collections.Generic;

namespace TimingApp.Data
{
	public static class Utils 
	{
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach(T item in enumeration)
			{
				action(item);
			}
		}
	}
}

