using UnityEngine;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Endgame;
using UnityEngine.UI;
using System.Linq;

public class ScoreboardCanvas : MonoBehaviour
{
	public int playerNameColWidth = 280;
	public int playerScoreColWidth = 70;
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
		listView.AddColumn ("Player Name", playerNameColWidth);
		listView.AddColumn ("Kills", playerScoreColWidth);
		listView.AddColumn ("Deaths", playerScoreColWidth);
		NetworkManager.onJoinedRoom += OnJoinedRoom;
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			canvas.enabled = !canvas.enabled;
		}
	}

	void OnJoinedRoom ()
	{
		UpdateScoreboard ();
	}

	void OnPlayerPropertiesChanged (PhotonPlayer player, Hashtable props)
	{
		Debug.Log ("Player properties changed");
		UpdateScoreboard ();
	}

	void UpdateScoreboard ()
	{
		Debug.Log ("Updatescoreboard");
		listView.ClearAllItems ();
		List<PhotonPlayer> players = PhotonNetwork.playerList.OrderByDescending (p => p.GetScore ())
			.ThenByDescending (p => p.GetDeaths ())
			.ToList ();
		foreach (var player in players) {
			Debug.Log (player.name);
			listView.AddItem (new string[]{
				player.name,
				player.GetScore ().ToString ("D"),
				player.GetDeaths ().ToString ("D")
			});
		}
	}
}
