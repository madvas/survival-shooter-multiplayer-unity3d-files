using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NetworkManager : Photon.MonoBehaviour
{
	
	public Text networkText;
	public static readonly int maxPlayersPerRoom = 5;
	public delegate void JoinedLobbyAction ();
	public event JoinedLobbyAction onJoinedLobby;

	public delegate void OnReceivedRoomListUpdateAction (List<Networking.Room> roomList);
	public static event OnReceivedRoomListUpdateAction onRoomListUpdate;

	public delegate void OnJoinedRoomAction ();
	public static event OnJoinedRoomAction onJoinedRoom;

	public delegate void OnLeftRoomAction ();
	public static event OnLeftRoomAction onLeftRoom;

	Vector3 position;
	Quaternion rotation;
	float smoothing = 10f;
	GameObject game;

	void Awake ()
	{
		game = GameObject.FindGameObjectWithTag ("Game");
	}

	void Start ()
	{
		PhotonNetwork.ConnectUsingSettings ("0.1");
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}
	
	// Update is called once per frame
	void Update ()
	{
		networkText.text = PhotonNetwork.connectionStateDetailed.ToString () + " players:" + PhotonNetwork.playerList.Length;	

		RoomInfo[] rooms = PhotonNetwork.GetRoomList ();
		networkText.text += "\n PlayerID: " + PhotonNetwork.player.ID;

		networkText.text += "\n";
		foreach (RoomInfo room in rooms)
			networkText.text += room.ToString () + "\n";

		networkText.text += "\n";
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			networkText.text += player.ToString () + "\n";
		}
	}
	
	void OnJoinedLobby ()
	{
		if (onJoinedLobby != null) {
			onJoinedLobby ();
		}
	}

	public List<Networking.Room> GetRoomList (int maxRooms, int maxPlayersPerRoom)
	{
		List<string> allRoomNames = new List<string> ();
		for (int i = 1; i <= maxRooms; i++) {
			allRoomNames.Add ("Room " + i);
		}

		List<Networking.Room> rooms = new List<Networking.Room> ();
		Debug.Log ("Existing rooms: " + PhotonNetwork.GetRoomList ().Length);
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList ()) {
			rooms.Add (new Networking.Room (roomInfo));
		}
		List<string> existingRoomNames = rooms.Select (room => room.name).ToList ();
		Debug.Log (existingRoomNames.ToString ());
		foreach (var item in existingRoomNames) {
			Debug.Log (item);
		}
		foreach (var item in allRoomNames.Except (existingRoomNames)) {
			Debug.Log (item);
		}
		Debug.Log (allRoomNames.Except (existingRoomNames).ToString ());
		foreach (string roomName in allRoomNames.Except(existingRoomNames)) {
			rooms.Add (new Networking.Room (roomName, (byte)maxPlayersPerRoom, 0));
		}
		return rooms;
	}

	public void JoinRoom (string roomName, string playerName)
	{
		Debug.Log ("joining room" + roomName);
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

	void OnReceivedRoomListUpdate ()
	{
		if (onRoomListUpdate != null) {
			onRoomListUpdate (GetRoomList (4, 5));
		}
	}
}
