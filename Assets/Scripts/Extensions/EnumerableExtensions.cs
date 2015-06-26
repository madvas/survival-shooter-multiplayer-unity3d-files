using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Collections;

public static class EnumerableExtension
{
	public static T PickRandom<T> (this IEnumerable<T> source)
	{
		return source.PickRandom (1).Single ();
	}
	
	public static IEnumerable<T> PickRandom<T> (this IEnumerable<T> source, int count)
	{
		return source.Shuffle ().Take (count);
	}
	
	public static IEnumerable<T> Shuffle<T> (this IEnumerable<T> source)
	{
		return source.OrderBy (x => Guid.NewGuid ());
	}

	public static IEnumerable<T> Debug<T> (this IEnumerable<T> source, string prefix = "")
	{
		foreach (var item in source) {
			UnityEngine.Debug.Log (prefix + " " + item);
		}
	}
}