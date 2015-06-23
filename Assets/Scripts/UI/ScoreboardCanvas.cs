using UnityEngine;
using System.Collections;

public class ScoreboardCanvas : MonoBehaviour
{
	Canvas canvas;

	void Awake ()
	{
		canvas = GetComponent<Canvas> ();
	}

	void Start ()
	{
		NetworkManager.onPlayerPropertiesChanged += OnPlayerPropertiesChanged;
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			canvas.enabled = !canvas.enabled;
		}
	}

	void OnPlayerPropertiesChanged (PhotonPlayer player, Hashtable props)
	{
		Debug.Log ("Player properties changed");
		Debug.Log (player);
		Debug.Log (props);
	}
}
