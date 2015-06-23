using UnityEngine;
using System.Collections;

public static class PlayerManager
{
	static readonly float respawnDelay = 2f;

	public delegate void OnInstantiatedAction (GameObject player);
	public static event OnInstantiatedAction onPlayerInstantiated;

	public static void SpawnPlayer ()
	{
		PositionData randomPosition = PositionManager.GetRandomSpawnPosition ();
		Debug.Log ("spawning player");
		GameObject player = PhotonNetwork.Instantiate ("Player", randomPosition.position, randomPosition.rotation, 0);
		if (onPlayerInstantiated != null) {
			Debug.Log ("PlayerManager onPlayerInstantiated");
			onPlayerInstantiated (player);
		}
	}

	public static void DestroyPlayer (GameObject player)
	{
		PhotonNetwork.Destroy (player);
		SpawnPlayer ();
	}
}