using UnityEngine;
using System.Collections;

public class SceneCamera : MonoBehaviour
{

	Camera camera;

	void Awake ()
	{
		camera = GetComponent<Camera> ();
		NetworkManager.onLeftRoom += OnLeftRoom;
		NetworkManager.onJoinedRoom += onJoinedRoom;
	}

	void OnLeftRoom ()
	{
		camera.enabled = true;
	}

	void onJoinedRoom ()
	{
		camera.enabled = false;
	}
}
