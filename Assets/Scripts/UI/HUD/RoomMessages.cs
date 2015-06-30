using UnityEngine;
using System.Collections;
using Endgame;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomMessages : Photon.MonoBehaviour
{

	InputField messageInput;
	bool isWriting = false;
	ScrollRect messagesScrollRect;
	Text messagesText;
	Color[] playerChatColors;

	void Awake ()
	{
		messageInput = GetComponentInChildren<InputField> ();
		messagesScrollRect = GetComponentInChildren<ScrollRect> ();
		messagesText = messagesScrollRect.content.gameObject.GetComponent<Text> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (isWriting) {
				photonView.RPC ("Chat", PhotonTargets.All, messageInput.text);
				messageInput.text = "";
				isWriting = false;
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
		AddMessage ("> " + newPlayer.name + " joined the room. " + GetPlayersInRoomString ());
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
	{
		AddMessage ("> " + otherPlayer.name + " left the room. " + GetPlayersInRoomString ());
	}

	void OnPlayerKill (object[] killData)
	{
		PhotonPlayer killer = killData [0] as PhotonPlayer;
		PhotonPlayer victim = killData [1] as PhotonPlayer;
		AddMessage (killer.name + " killed " + victim.name);
	}

	string GetPlayersInRoomString ()
	{
		return string.Format ("({0}/{1})", PhotonNetwork.room.playerCount, PhotonNetwork.room.maxPlayers);
	}

	[PunRPC]
	void Chat (string newLine, PhotonMessageInfo mi)
	{
		AddMessage (string.Format ("<b>{0}</b> {1}", mi.sender.name, newLine));
	}

	void string GetPlayerChatColor(PhotonPlayer player) {
		return playerChatColors[player.GetMaterialIndex()].ToHexStringRGB();
	}
}
