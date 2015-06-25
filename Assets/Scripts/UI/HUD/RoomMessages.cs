using UnityEngine;
using System.Collections;
using Endgame;

[RequireComponent(typeof(PhotonView))]
public class RoomMessages : Photon.MonoBehaviour
{

	ListView listView;
	int messagesWidth;

	void Awake ()
	{
		listView = GetComponent<ListView> ();
		messagesWidth = (int)GetComponent<RectTransform> ().rect.width;
	}

	void Start ()
	{
		listView.AddColumn ("Messages", messagesWidth);
		listView.ShowColumnHeaders = false;
		AddEmptyMessages ();
	}

	void AddEmptyMessages ()
	{
		for (int i = 0; i < 5; i++) {
			listView.AddItem ("");
		}
	}

	void AddMessage (string message)
	{
		listView.AddItem (message);
		listView.SetVerticalScrollBarValue (9999f);
	}

	void OnJoinedRoom ()
	{
		listView.ClearAllItems ();
		OnPhotonPlayerConnected (PhotonNetwork.player);
	}

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		AddMessage ("Player " + newPlayer.name + " joined the room. " + GetPlayersInRoomString ());
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
	{
		AddMessage ("Player " + otherPlayer.name + " left the room. " + GetPlayersInRoomString ());
	}

	string GetPlayersInRoomString ()
	{
		return string.Format ("({0}/{1})", PhotonNetwork.room.playerCount, PhotonNetwork.room.maxPlayers);
	}	
}
