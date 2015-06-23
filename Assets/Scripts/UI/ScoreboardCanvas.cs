using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Endgame;
using UnityEngine.UI;

public class ScoreboardCanvas : MonoBehaviour
{
	Canvas canvas;
	ListView listView;

	void Awake ()
	{
		canvas = GetComponent<Canvas> ();
		listView = GetComponentInChildren<ListView> ();
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
