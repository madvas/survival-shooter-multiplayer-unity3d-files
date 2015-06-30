using Sytem.Collections;
using UnityEngine;

public static class StringExtensions
{
	public static string Colorize (this string text, Color color)
	{
		return string.Format ("<color=#{0}>{1}</color>", color.ToHexStringRGB (), text);
	}
}