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
	RoomTimeManager roomTimeManager;

	void Awake ()
	{
		canvas = GetComponent<Canvas> ();
		listView = GetComponentInChildren<ListView> ();
		roomTimeManager = GameObject.FindGameObjectWithTag ("RoomTimeManager").GetComponent<RoomTimeManager> ();
	}

	void Start ()
	{
		NetworkManager.onPlayerPropertiesChanged += OnPlayerPropertiesChanged;
		listView.AddColumn ("Player Name", playerNameColWidth);
		listView.AddColumn ("Kills", playerScoreColWidth);
		listView.AddColumn ("Deaths", playerScoreColWidth);
	}
	
	void Update ()
	{
		if (!roomTimeManager.isPauseState ()) {
			canvas.enabled = PhotonNetwork.inRoom && Input.GetKey (KeyCode.Tab);
		}
	}

	void OnJoinedRoom ()
	{
		UpdateScoreboard ();
	}

	void OnLeftRoom ()
	{
		canvas.enabled = false;
	}

	void OnPlayerPropertiesChanged (PhotonPlayer player, Hashtable props)
	{
		UpdateScoreboard ();
	}

	void OnPauseStarted ()
	{
		canvas.enabled = true;
	}

	void OnRoundStarted ()
	{
		canvas.enabled = false;
	}

	void OnJoinedRoomInPause ()
	{
		canvas.enabled = true;
	}

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		UpdateScoreboard ();
	}

	void UpdateScoreboard ()
	{
		listView.ClearAllItems ();
		List<PhotonPlayer> players = PhotonNetwork.playerList.OrderByDescending (p => p.GetScore ())
			.ThenByDescending (p => p.GetDeaths ())
			.ToList ();
		foreach (var player in players) {
			listView.AddItem (new string[]{
				player.name,
				player.GetScore ().ToString ("D"),
				player.GetDeaths ().ToString ("D")
			});
		}
	}
}
