using UnityEngine;
using System.Collections;

public class SceneCamera : MonoBehaviour
{

	Camera camera;
	AudioListener audioListener;

	void Awake ()
	{
		camera = GetComponent<Camera> ();
		audioListener = GetComponent<AudioListener> ();
	}

	void OnLeftRoom ()
	{
		SetEnabled (true);
	}

	void OnJoinedRoomInRound ()
	{
		SetEnabled (false);
	}

	void OnJoinedRoomInPause ()
	{
		SetEnabled (true);
	}

	void OnPauseStarted ()
	{
		SetEnabled (true);
	}

	void OnRoundStarted ()
	{
		SetEnabled (false);
	}

	void SetEnabled (bool enabled)
	{
		camera.enabled = enabled;
		audioListener.enabled = enabled;
	}
}
