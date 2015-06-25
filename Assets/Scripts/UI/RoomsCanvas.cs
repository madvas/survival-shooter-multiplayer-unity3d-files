using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Endgame;
using UnityEngine.UI;

public class RoomsCanvas : MonoBehaviour
{
	public ListView listView;
	public int roomNameColWidth = 278;
	public int roomPlayersColWidth = 100;
	public GameObject joinButton;
	public int nickMaxLength = 15;
	public int nickMinLength = 1;

	[SerializeField]
	InputField
		nicknameField;

	Button joinBtnScript;
	List<Networking.Room> rooms;
	Canvas canvas;
	NetworkManager networkManager;

	void Awake ()
	{
		joinBtnScript = joinButton.GetComponent<Button> ();
		canvas = transform.GetComponent<Canvas> ();
		networkManager = GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<NetworkManager> ();
	}

	void Start ()
	{
		nicknameField.text = "Shooter" + Random.Range (1000, 9999);
		listView.AddColumn ("Name", roomNameColWidth);
		listView.AddColumn ("Players", roomPlayersColWidth);
		validateRoomForm ();
	}

	void OnEnable ()
	{
		listView.SelectedIndexChanged += OnSelectedItemChange;
		listView.ItemActivate += OnItemActivate;
	}
	
	
	void OnDisable ()
	{
		listView.SelectedIndexChanged -= OnSelectedItemChange;
		listView.ItemActivate -= OnItemActivate;
	}

	void OnJoinedRoom ()
	{
		canvas.enabled = false;
	}

	void OnJoinedLobby ()
	{
		canvas.enabled = true;
		validateRoomForm ();
	}

	public void JoinSelectedRoom ()
	{
		if (listView.SelectedItems.Count > 0) {
			Networking.Room selectedRoom = rooms [listView.SelectedItems [0].Index];
			networkManager.JoinRoom (selectedRoom.name, nicknameField.text);
		}
	}

	public void OnNicknameTextChange ()
	{
		validateRoomForm ();
	}
	
	public bool validateRoomForm ()
	{
		if (nicknameField.text.Length < nickMinLength || nicknameField.text.Length > nickMaxLength 
			|| listView.SelectedItems.Count <= 0 || isSelectedRoomFull ()) {
			disableJoining ();
			return false;
		} 
		enableJoining ();
		return true;
	}

	void AddListItem (string roomName, int playerCount, int maxPlayers)
	{
		string[] item = new string[]{
			roomName,
			playerCount.ToString ("D") + "/" + maxPlayers,
		};
		listView.AddItem (item);
	}

	void disableJoining ()
	{
		joinBtnScript.interactable = false;
	}

	void enableJoining ()
	{
		joinBtnScript.interactable = true;
	}

	private void OnSelectedItemChange (object sender, System.EventArgs args)
	{
		validateRoomForm ();
	}

	private void OnItemActivate (object sender, System.EventArgs args)
	{
		if (validateRoomForm ()) {
			JoinSelectedRoom ();
		}
	}

	private void OnRoomListUpdate (List<Networking.Room> roomList)
	{
		rooms = roomList;
		listView.ClearAllItems ();
		foreach (var room in roomList) {
			AddListItem (room.name, room.playerCount, room.maxPlayers);
		}
	}

	private bool isSelectedRoomFull ()
	{
		Networking.Room selectedRoom = rooms [listView.SelectedItems [0].Index];
		return selectedRoom.maxPlayers <= selectedRoom.playerCount;
	}
}
