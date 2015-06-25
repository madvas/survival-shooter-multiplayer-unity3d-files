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
		AddEmptyMessages ();
		InvokeRepeating ("AddRandomMsg", 1, 1);
	}

	void AddEmptyMessages ()
	{
		for (int i = 0; i < 5; i++) {
			listView.AddItem ("");
		}
	}

	void AddMessage (string message)
	{
		listView.AddItem (message);
		listView.SetVerticalScrollBarValue (9999f);
	}

	void AddRandomMsg ()
	{
		AddMessage ("Player " + Random.Range (1000, 9999) + " just killed me");
	}
}
