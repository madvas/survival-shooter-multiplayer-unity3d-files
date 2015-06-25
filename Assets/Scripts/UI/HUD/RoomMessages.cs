using UnityEngine;
using System.Collections;
using Endgame;

public class RoomMessages : MonoBehaviour
{

	ListView listView;

	void Awake ()
	{
		listView = GetComponent<ListView>();
	}

	void Start () {
		listView.AddItem(new string[] {
			"test"
		})
	}
}
