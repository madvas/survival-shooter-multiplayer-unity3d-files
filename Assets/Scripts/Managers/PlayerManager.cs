using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

		List<int> availableMaterials = playerMaterials.ToList ().Except (PhotonNetwork.playerList.GetMaterials ());

		Debug.Log (PhotonNetwork.playerList.GetMaterials ().Count);
		Debug.Log (availableMaterials.Count);
		int materialIndex = Random.Range (0, availableMaterials.Count);

		PhotonNetwork.player.SetMaterialIndex (materialIndex);
		body.material = availableMaterials [materialIndex];

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}
}