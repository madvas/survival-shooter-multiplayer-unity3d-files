using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	public float respawnDelay = 2f;
	public Material[] playerMaterials;

	void OnJoinedRoom ()
	{
		InstantiatePlayer ();
	}

	void InstantiatePlayer ()
	{
		GameObject player = PhotonNetwork.Instantiate ("Player", Vector3.zero, Quaternion.identity, 0);
		SkinnedMeshRenderer body = player.FindComponentInChildWithTag<SkinnedMeshRenderer> ("PlayerBodyMesh");

		int materialIndex = Random.Range (0, playerMaterials.Length);
		body.material = playerMaterials [materialIndex];

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}
}