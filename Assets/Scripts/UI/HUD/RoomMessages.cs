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
		listView.AddItem ("Player bla joined game");
		listView.AddItem ("Player blablasbas joined game");
	}
}
