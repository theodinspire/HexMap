using System.Collections.Generic;

namespace Extensions
{
	public static class ListExtensions
	{
		public static void Add<T>(this List<T> list, params T[] items)
		{
			list.AddRange(items);
		}
	}
}