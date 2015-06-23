namespace Endgame
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Reflection;

	static class HelperFunctions
	{
		public static T CallPrivateMethod<T>(object objectContainingMethod, string methodName, params object[] parameters)
		{
			MethodInfo methodInfo = objectContainingMethod.GetType().GetMethod
			(
				methodName,
				BindingFlags.NonPublic | BindingFlags.Instance
			);

			object[] parameterArray = new object[] { };
			if (parameters != null)
			{
				parameterArray = parameters;
			}

			object result = methodInfo.Invoke(objectContainingMethod, parameterArray);
			return (T)result;
		}

		public static void CallPrivateMethod(object objectContainingMethod, string methodName, params object[] parameters)
		{
			MethodInfo methodInfo = objectContainingMethod.GetType().GetMethod
			(
				methodName,
				BindingFlags.NonPublic | BindingFlags.Instance
			);

			object[] parameterArray = new object[] { };
			if (parameters != null)
			{
				parameterArray = parameters;
			}

			methodInfo.Invoke(objectContainingMethod, parameterArray);
		}

		public static void Repeat(int count, Action action)
		{
			for (int i = 0; i < count; i++)
			{
				action();
			}
		}
	}
}
