using UnityEngine;
using System.Collections;
using Endgame;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomMessages : Photon.MonoBehaviour
{

	ListView listView;
	int messagesWidth;
	InputField messageInput;
	bool isWriting = false;

	void Awake ()
	{
		listView = GetComponentInChildren<ListView> ();
		messagesWidth = (int)GetComponent<RectTransform> ().rect.width;
		messageInput = GetComponentInChildren<InputField> ();
	}

	void Start ()
	{
		listView.AddColumn ("Messages", messagesWidth);
		listView.ShowColumnHeaders = false;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (isWriting) {
				messageInput.text = "";
				GameObjectHelper.SendMessageToAll ("OnWritingMesssageEnded");
			} else {
				messageInput.ActivateInputField ();
				isWriting = true;
				GameObjectHelper.SendMessageToAll ("OnWritingMesssageStarted");
			}
		}
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
		AddEmptyMessages ();
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
