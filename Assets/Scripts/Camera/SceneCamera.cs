using UnityEngine;
using System.Collections;

public class SceneCamera : MonoBehaviour
{

	Camera camera;

	void Awake ()
	{
		camera = GetComponent<Camera> ();
	}

	void OnLeftRoom ()
	{
		camera.enabled = true;
	}

	void OnJoinedRoom ()
	{
		camera.enabled = false;
	}
}
