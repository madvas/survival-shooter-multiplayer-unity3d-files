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
		messagesWidth = GetComponent<RectTransform>().rect.width;
	}

	void Start ()
	{
		listView.AddColumn ("Messages", messagesWidth);
		listView.ShowColumnHeaders = false;
		listView.AddItem ("test");
	}

	void AddMessage
}
