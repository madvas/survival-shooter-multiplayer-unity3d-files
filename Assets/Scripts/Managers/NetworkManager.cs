using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : Photon.MonoBehaviour
{
	
	public int maxPlayersPerRoom = 5;
	public int maxRooms = 4;
	public delegate void JoinedLobbyAction ();
	public event JoinedLobbyAction onJoinedLobby;

	public delegate void OnReceivedRoomListUpdateAction (List<Networking.Room> roomList);
	public static event OnReceivedRoomListUpdateAction onRoomListUpdate;

	public delegate void OnJoinedRoomAction ();
	public static event OnJoinedRoomAction onJoinedRoom;

	public delegate void OnLeftRoomAction ();
	public static event OnLeftRoomAction onLeftRoom;

	public delegate void OnPhotonPlayerPropertiesChangedAction (PhotonPlayer player,Hashtable props);
	public static event OnPhotonPlayerPropertiesChangedAction onPlayerPropertiesChanged;

	void Start ()
	{
		PhotonNetwork.ConnectUsingSettings ("0.1");
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}
	
	void OnJoinedLobby ()
	{
		if (onJoinedLobby != null) {
			onJoinedLobby ();
		}
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

	public static void JoinRoom (string roomName, string playerName)
	{
		Debug.Log ("joining room" + roomName);
		if (playerName.Length <= 0 || roomName.Length <= 0) {
			return;
		}
		RoomOptions roomOptions = new RoomOptions (){ 
			isVisible = true, 
			maxPlayers = (byte)maxPlayersPerRoom 
		};
		Debug.Log ("Joining as player " + playerName);
		PhotonNetwork.player.name = playerName;
		PhotonNetwork.JoinOrCreateRoom (roomName, roomOptions, TypedLobby.Default);
	}

	public void LeaveRoom ()
	{
		PhotonNetwork.LeaveRoom ();
	}
	
	void OnJoinedRoom ()
	{
		Debug.Log ("Room joined");
		if (onJoinedRoom != null) {
			onJoinedRoom ();
		}
	}

	void OnLeftRoom ()
	{
		if (onLeftRoom != null) {
			Debug.Log ("room left");
			onLeftRoom ();
		}
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		if (onPlayerPropertiesChanged != null) {
			PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
			Hashtable props = playerAndUpdatedProps [1] as Hashtable;
			onPlayerPropertiesChanged (player, props);
		}
	}

	void OnReceivedRoomListUpdate ()
	{
		if (onRoomListUpdate != null) {
			onRoomListUpdate (GetRoomList (maxRooms, maxPlayersPerRoom));
		}
	}
}
