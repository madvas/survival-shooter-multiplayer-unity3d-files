using UnityEngine;
using System.Collections.Generic;

public class ComponentHelper : MonoBehaviour
{

	public static void sendMonoMessage (string methodName, params object[] parameters)
	{
		HashSet<GameObject> objectsToCall;
		objectsToCall = PhotonNetwork.FindGameObjectsWithComponent (typeof(MonoBehaviour));
		
		object callParameter = (parameters != null && parameters.Length == 1) ? parameters [0] : parameters;
		foreach (GameObject gameObject in objectsToCall) {
			gameObject.SendMessage (methodName, callParameter, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static HashSet<GameObject> FindGameObjectsWithComponent (Type type)
	{
		HashSet<GameObject> objectsWithComponent = new HashSet<GameObject> ();
		
		Component[] targetComponents = (Component[])GameObject.FindObjectsOfType (type);
		for (int index = 0; index < targetComponents.Length; index++) {
			objectsWithComponent.Add (targetComponents [index].gameObject);
		}
		
		return objectsWithComponent;
	}
}
