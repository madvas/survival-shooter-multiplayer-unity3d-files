using UnityEngine;
using System.Collections.Generic;
using Endgame;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomMessages : Photon.MonoBehaviour
{
	public Color playerChatColor0;
	public Color playerChatColor1;
	public Color playerChatColor2;
	public Color playerChatColor3;
	public Color playerChatColor4;
	public Color playerChatColor5;
	public Color playerChatColor6;

	InputField messageInput;
	bool isWriting = false;
	ScrollRect messagesScrollRect;
	Text messagesText;
	List<Color> playerChatColors;

	void Awake ()
	{
		messageInput = GetComponentInChildren<InputField> ();
		messagesScrollRect = GetComponentInChildren<ScrollRect> ();
		messagesText = messagesScrollRect.content.gameObject.GetComponent<Text> ();
	}

	void Start ()
	{
		Debug.LogError ("Roommessages started");
		Debug.Log (playerChatColor0);
//		playerChatColors.Add (playerChatColor0);
//		playerChatColors.Add (playerChatColor1);
//		playerChatColors.Add (playerChatColor2);
//		playerChatColors.Add (playerChatColor3);
//		playerChatColors.Add (playerChatColor4);
//		playerChatColors.Add (playerChatColor5);
//		playerChatColors.Add (playerChatColor6);
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
