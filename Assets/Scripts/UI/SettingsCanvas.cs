using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsCanvas : MonoBehaviour
{

	Canvas canvas;

	void Start ()
	{
		canvas = GetComponent<Canvas> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			canvas.enabled = !canvas.enabled;
		}
	}

	public void DisableSound ()
	{

	}
}
