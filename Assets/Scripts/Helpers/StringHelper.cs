using System.Collections;
using UnityEngine;

public static class StringHelper
{
	public static string Colorize (string text, Color color)
	{
		return string.Format ("<color=#{0}>{1}</color>", color.ToHexStringRGB (), text);
	}
}