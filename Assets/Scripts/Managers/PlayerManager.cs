using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	public float respawnDelay = 2f;

	void OnJoinedRoom ()
	{
		InstantiatePlayer ();
	}

	void InstantiatePlayer ()
	{
		PositionData randomPosition = PositionHelper.GetRandomSpawnPosition ();
		Debug.Log ("InstantiatePlayer");
		Debug.Log (randomPosition.position);
		GameObject player = PhotonNetwork.Instantiate ("Player", randomPosition.position, randomPosition.rotation, 0);
		player.GetComponentInChildren<PlayerShooting> ().enabled = true;
		player.GetComponent<PlayerMovement> ().enabled = true;
		player.GetComponent<PlayerSpawning> ().enabled = true;
		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnPlayerInstantiated", player);
	}
}