using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public static class EnumerableExtension
{
	public static T PickRandom<T> (this List<T> source)
	{
		return source [Random.Range (0, source.Count)];
	}
}