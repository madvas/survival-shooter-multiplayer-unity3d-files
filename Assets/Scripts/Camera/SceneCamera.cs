﻿using UnityEngine;
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
		Debug.Log ("Scenecamera OnJoinedRoomInPause");
		camera.enabled = false;
	}

	void OnPauseStarted ()
	{
		camera.enabled = true;
	}

	void OnRoundStarted ()
	{
		camera.enabled = false;
	}

	void OnJoinedRoomInPause ()
	{
		Debug.Log ("SceneCamera OnJoinedRoomInPause");
		camera.enabled = true;
	}
}
