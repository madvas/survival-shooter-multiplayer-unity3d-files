using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

	void Awake ()
	{
		NetworkManager.onLeftRoom += OnLeftRoom;
		NetworkManager.onJoinedRoom += onJoinedRoom;
	}
	
	void OnLeftRoom ()
	{
	}
	
	void onJoinedRoom ()
	{
		PlayerManager.SpawnPlayer ();
	}
}
