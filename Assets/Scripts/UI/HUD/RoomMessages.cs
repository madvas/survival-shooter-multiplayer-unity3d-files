using UnityEngine;
using System.Collections;
using Endgame;

public class RoomMessages : MonoBehaviour
{

	ListView listView;

	void Awake ()
	{
		listView = GetComponent<ListView> ();
	}

	void Start ()
	{
		listView.AddColumn ("new Col", 250);
		string[] item = new string[]{
			"test"
		};
		listView.AddItem (item);
	}
}
