using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Endgame;
using UnityEngine.UI;

public class RoomsPanel : MonoBehaviour
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
	NetworkManager networkManager;
	List<Networking.Room> rooms;
	Canvas canvas;

	void Awake ()
	{
		joinBtnScript = joinButton.GetComponent<Button> ();
		networkManager = GetComponent<NetworkManager> ();
		canvas = transform.GetComponent<Canvas> ();
	}

	void Start ()
	{
		Debug.Log ("STart here");
		nicknameField.text = "Shooter" + Random.Range (1000, 9999);
		Debug.Log ("Shooter" + Random.Range (1000, 9999));
		AddColumn ("Name");
		AddColumn ("Players");
		listView.Columns [0].Width = roomNameColWidth;
		listView.Columns [1].Width = roomPlayersColWidth;
		validateRoomForm ();
	}

	void OnEnable ()
	{
		listView.SelectedIndexChanged += OnSelectedItemChange;
		listView.ItemActivate += OnItemActivate;
		NetworkManager.onRoomListUpdate += onRoomListUpdate;
	}
	
	
	void OnDisable ()
	{
		listView.SelectedIndexChanged -= OnSelectedItemChange;
		listView.ItemActivate -= OnItemActivate;
		NetworkManager.onRoomListUpdate -= onRoomListUpdate;
	}

	public void JoinSelectedRoom ()
	{
		Debug.Log (nicknameField.text);
		Networking.Room selectedRoom = rooms [listView.SelectedItems [0].Index];
		Debug.Log (selectedRoom.name);
		networkManager.JoinRoom (selectedRoom.name, nicknameField.text);
		canvas.enabled = false;
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

	void AddColumn (string title)
	{
		ColumnHeader column = new ColumnHeader ();
		column.Text = title;
		listView.Columns.Add (column);
	}

	void AddListItem (string roomName, int playerCount, int maxPlayers)
	{
		string[] item = new string[]{
			roomName,
			playerCount.ToString ("D") + "/" + maxPlayers,
		};
		listView.Items.Add (new ListViewItem (item));
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

	private void onRoomListUpdate (List<Networking.Room> roomList)
	{
		rooms = roomList;
		int itemsCount = listView.Items.Count;
		for (int i = 0; i < itemsCount; i++) {
			listView.Items.RemoveAt (0);
		}
		Debug.Log (roomList);
		foreach (var room in roomList) {
			Debug.Log ("Adding row " + room.name + " " + room.playerCount);
			AddListItem (room.name, room.playerCount, room.maxPlayers);
		}
	}

	private bool isSelectedRoomFull ()
	{
		Networking.Room selectedRoom = rooms [listView.SelectedItems [0].Index];
		return NetworkManager.maxPlayersPerRoom <= selectedRoom.playerCount;
	}

}
