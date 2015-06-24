using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	void OnJoinedRoom ()
	{
		PlayerManager.SpawnPlayer ();
	}
}
