using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	public float respawnDelay = 2f;

	public static void InstantiatePlayer ()
	{
		PositionData randomPosition = PositionHelper.GetRandomSpawnPosition ();
		Debug.Log ("spawning player");
		GameObject player = PhotonNetwork.Instantiate ("Player", randomPosition.position, randomPosition.rotation, 0);
		GameObjectHelper.SendMessageToAll ("OnPlayerInstantiated", player);
	}
}