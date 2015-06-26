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

		int[] availableMaterials = Enumerable.Range (0, playerMaterials.Length).Except (PhotonNetwork.playerList.GetMaterials ()).ToArray ();

		Debug.Log (PhotonNetwork.playerList.GetMaterials ().Count);
		int materialIndex = availableMaterials.PickRandom ();


		foreach (var item in PhotonNetwork.playerList.GetMaterials ()) {
			Debug.Log ("used mat: " + item);
		}

		Debug.Log (availableMaterials.Length);
		Debug.Log (materialIndex);
		Debug.Log (playerMaterials [materialIndex]);

		PhotonNetwork.player.SetMaterialIndex (materialIndex);
		body.material = playerMaterials [materialIndex];

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}
}