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
		AddMessage (string.Format ("> {0} joined the room {1}", GetPlayerColoredName (newPlayer), GetPlayersInRoomString ()));
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
	{
		AddMessage (string.Format ("> {0} left the room {1}", GetPlayerColoredName (otherPlayer), GetPlayersInRoomString ()));
	}

	void OnPlayerKill (object[] killData)
	{
		PhotonPlayer killer = killData [0] as PhotonPlayer;
		PhotonPlayer victim = killData [1] as PhotonPlayer;
		AddMessage (string.Format ("> {0} killed {1}", GetPlayerColoredName (killer), GetPlayerColoredName (victim)));
	}

	string GetPlayersInRoomString ()
	{
		return string.Format ("({0}/{1})", PhotonNetwork.room.playerCount, PhotonNetwork.room.maxPlayers);
	}

	[PunRPC]
	void Chat (string newLine, PhotonMessageInfo mi)
	{
		AddMessage (string.Format ("<b>{0}</b> {1}", GetPlayerColoredName (mi.sender), newLine));
	}

	string GetPlayerChatColor (PhotonPlayer player)
	{
		return playerChatColors [player.GetMaterialIndex ()].ToHexStringRGB ();
	}

	string GetPlayerColoredName (PhotonPlayer player)
	{
		return string.Format ("<color=#{0}>{1}</color>", GetPlayerChatColor (player), player.name);
	}
}
