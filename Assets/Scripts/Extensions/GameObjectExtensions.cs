using UnityEngine;

public static class MyGameObjectExtensions
{
	public static T FindComponentInChildWithTag<T> (this GameObject parent, string tag)where T:Component
	{
		Transform t = parent.transform;
		foreach (Transform tr in t) {
			if (tr.tag == tag) {
				return tr.GetComponent<T> ();
			}
		}
		return null;
	}

	public static void SendMessageMultiArg (this GameObject gameObject, string methodName, params object[] parameters)
	{
		gameObject.SendMessage (methodName, parameters, SendMessageOptions.DontRequireReceiver);
	}

	public static void BroadcastMessageMultiArg (this GameObject gameObject, string methodName, params object[] parameters)
	{
		gameObject.BroadcastMessage (methodName, parameters, SendMessageOptions.DontRequireReceiver);
	}


}