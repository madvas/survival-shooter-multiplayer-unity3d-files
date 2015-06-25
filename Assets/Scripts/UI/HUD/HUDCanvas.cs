using UnityEngine;
using System.Collections;

public class HUDCanvas : MonoBehaviour
{
	Canvas canvas;
	
	void Awake ()
	{
		canvas = GetComponent<Canvas> ();
	}
	
	void OnLeftRoom ()
	{
		canvas.enabled = false;
	}
	
	void OnJoinedRoomInRound ()
	{
		canvas.enabled = true;
	}
	
	void OnJoinedRoomInPause ()
	{
		canvas.enabled = false;
	}
	
	void OnPauseStarted ()
	{
		canvas.enabled = false;
	}
	
	void OnRoundStarted ()
	{
		canvas.enabled = true;
	}
}
