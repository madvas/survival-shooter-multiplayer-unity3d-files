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
	PlayerManager playerManager;

	void Awake ()
	{
		canvas = GetComponent<Canvas> ();
		listView = GetComponentInChildren<ListView> ();
		roomTimeManager = GameObject.FindGameObjectWithTag ("RoomTimeManager").GetComponent<RoomTimeManager> ();
		playerManager = GameObject.FindGameObjectWithTag ("PlayerManager").GetComponent<PlayerManager> ();
	}

	void Start ()
	{
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

	void OnJoinedRoomInPause ()
	{
		canvas.enabled = true;
	}

	void OnLeftRoom ()
	{
		canvas.enabled = false;
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
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

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		UpdateScoreboard ();
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
	{
		UpdateScoreboard ();
	}

	void UpdateScoreboard ()
	{
		listView.ClearAllItems ();
		List<PhotonPlayer> players = PhotonNetwork.playerList
			.OrderByDescending (p => p.GetScore ())
			.ThenBy (p => p.GetDeaths ())
			.ToList ();
		foreach (var player in players) {
			Color textColor = playerManager.GetPlayerTextColor (player);
			listView.AddItem (player.name, player.GetScore ().ToString ("D"), player.GetDeaths ().ToString ("D"));
		}
	}

	void WrapInPlayerColor (string Text)
	{

	}
}
