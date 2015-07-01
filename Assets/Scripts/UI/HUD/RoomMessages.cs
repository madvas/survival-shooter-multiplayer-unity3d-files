using UnityEngine;
using System.Collections.Generic;
using Endgame;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomMessages : Photon.MonoBehaviour
{	
	InputField messageInput;
	bool isWriting = false;
	ScrollRect messagesScrollRect;
	Text messagesText;
	PlayerManager playerManager;

	void Awake ()
	{
		messageInput = GetComponentInChildren<InputField> ();
		messagesScrollRect = GetComponentInChildren<ScrollRect> ();
		messagesText = messagesScrollRect.content.gameObject.GetComponent<Text> ();
		playerManager = GameObject.FindGameObjectWithTag ("PlayerManager").GetComponent<PlayerManager> ();
	}

	void Update ()
	{
		Event e = Event.current;
		if (e != null) {
			Debug.Log (e);
		}

//		if (e && e.Equals (Event.KeyboardEvent ("[enter]"))) {
//			if (isWriting) {
//				if (messageInput.text.Length > 0) {
//					photonView.RPC ("Chat", PhotonTargets.All, messageInput.text);
//				}
//				messageInput.text = "";
//				isWriting = false;
//				messageInput.DeactivateInputField ();
//				GameObjectHelper.SendMessageToAll ("OnWritingMesssageEnded");
//			} else {
//				messageInput.ActivateInputField ();
//				isWriting = true;
//				GameObjectHelper.SendMessageToAll ("OnWritingMesssageStarted");
//			}
//		}
	}

	void OnGUI ()
	{
		Event e = Event.current;
		if (e.type == EventType.keyDown && e.keyCode == KeyCode.Return) {
			if (isWriting) {
				if (messageInput.text.Length > 0) {
					photonView.RPC ("Chat", PhotonTargets.All, messageInput.text);
				}
				messageInput.text = "";
				isWriting = false;
				messageInput.DeactivateInputField ();
				GameObjectHelper.SendMessageToAll ("OnWritingMesssageEnded");
			} else {
				messageInput.ActivateInputField ();
				isWriting = true;
				GameObjectHelper.SendMessageToAll ("OnWritingMesssageStarted");
			}
		}
	}

	void AddMessage (string message)
	{
		messagesText.text += message + "\n";
	}

	void OnJoinedRoom ()
	{
		messagesText.text = "";
		OnPhotonPlayerConnected (PhotonNetwork.player);
	}

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		AddMessage (string.Format ("> {0} joined the room {1}", playerManager.GetPlayerColoredName (newPlayer), GetPlayersInRoomString ()));
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
	{
		AddMessage (string.Format ("> {0} left the room {1}", playerManager.GetPlayerColoredName (otherPlayer), GetPlayersInRoomString ()));
	}

	void OnPlayerKill (object[] killData)
	{
		PhotonPlayer killer = killData [0] as PhotonPlayer;
		PhotonPlayer victim = killData [1] as PhotonPlayer;
		AddMessage (string.Format ("> {0} killed {1}", playerManager.GetPlayerColoredName (killer), playerManager.GetPlayerColoredName (victim)));
	}

	string GetPlayersInRoomString ()
	{
		return string.Format ("({0}/{1})", PhotonNetwork.room.playerCount, PhotonNetwork.room.maxPlayers);
	}

	[PunRPC]
	void Chat (string newLine, PhotonMessageInfo mi)
	{
		AddMessage (string.Format ("{0} {1}", playerManager.GetPlayerColoredName (mi.sender), newLine));
	}
}
