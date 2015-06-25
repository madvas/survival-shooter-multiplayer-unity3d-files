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

	void OnJoinedRoomInRound ()
	{
		camera.enabled = false;
	}

	void OnJoinedRoomInPause ()
	{
		camera.enabled = true;
	}

	void OnPauseStarted ()
	{
		camera.enabled = true;
	}

	void OnRoundStarted ()
	{
		camera.enabled = false;
	}
}
