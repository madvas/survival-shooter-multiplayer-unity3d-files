using UnityEngine;
using System.Collections;

public class ScoreboardCanvas : MonoBehaviour
{
	Canvas canvas;
	
	void Start ()
	{
		canvas = GetComponent<Canvas> ();
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			canvas.enabled = !canvas.enabled;
		}
	}
}
