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
	
	void onJoinedRoom ()
	{
		canvas.enabled = true;
	}
}
