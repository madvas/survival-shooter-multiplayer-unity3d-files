using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviour
{
	
	public int maxPlayersPerRoom = 5;
	public int maxRooms = 4;

	void Start ()
	{
//		PhotonNetwork.offlineMode = true; 
		PhotonNetwork.ConnectUsingSettings ("0.1");
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}
	
	void OnJoinedLobby ()
	{
//		JoinRoom ("Room 1", "Shooter" + Random.Range (1000, 9999));
	}

	public List<Networking.Room> GetRoomList (int maxRooms, int maxPlayersPerRoom)
	{
		List<string> allRoomNames = new List<string> ();
		for (int i = 1; i <= maxRooms; i++) {
			allRoomNames.Add ("Room " + i);
		}

		List<Networking.Room> rooms = new List<Networking.Room> ();
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList ()) {
			rooms.Add (new Networking.Room (roomInfo));
		}
		List<string> existingRoomNames = rooms.Select (room => room.name).ToList ();
		foreach (string roomName in allRoomNames.Except(existingRoomNames)) {
			rooms.Add (new Networking.Room (roomName, (byte)maxPlayersPerRoom, 0));
		}
		return rooms;
	}

	public void JoinRoom (string roomName, string playerName)
	{
		if (playerName.Length <= 0 || roomName.Length <= 0) {
			return;
		}
		RoomOptions roomOptions = new RoomOptions (){ 
			isVisible = true, 
			maxPlayers = (byte)maxPlayersPerRoom 
		};
		PhotonNetwork.player.name = playerName;
		PhotonNetwork.JoinOrCreateRoom (roomName, roomOptions, TypedLobby.Default);
	}

	public void LeaveRoom ()
	{
		PhotonNetwork.LeaveRoom ();
	}
	
	void OnReceivedRoomListUpdate ()
	{
		GameObjectHelper.SendMessageToAll ("OnRoomListUpdate", GetRoomList (maxRooms, maxPlayersPerRoom));
	}
}
