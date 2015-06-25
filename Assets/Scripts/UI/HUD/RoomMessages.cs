using UnityEngine;
using System.Collections;
using Endgame;

public class RoomMessages : MonoBehaviour
{

	ListView listView;
	int messagesWidth;

	void Awake ()
	{
		listView = GetComponent<ListView> ();
		messagesWidth = (int)GetComponent<RectTransform> ().rect.width;
	}

	void Start ()
	{
		listView.AddColumn ("Messages", messagesWidth);
		listView.ShowColumnHeaders = false;
		AddMessage ("Player asdasd joined the game");
		AddMessage ("Player asdasd left the game");
	}

	void AddMessage (string message)
	{
		listView.AddItem (message);
		listView.SetVerticalScrollBarValue (9999f);
	}
}
