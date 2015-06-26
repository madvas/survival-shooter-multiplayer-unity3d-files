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

		List<int> availableMaterials = Enumerable.Range (0, playerMaterials.Length).ToList ().Except (PhotonNetwork.playerList.GetMaterials ()).ToList ();

		foreach (var item in Enumerable.Range (0, playerMaterials.Length).ToList ()) {
			Debug.Log ("used mat: " + item);
		}

		int materialIndex = availableMaterials.PickRandom ();

		PhotonNetwork.player.SetMaterialIndex (materialIndex);
		body.material = playerMaterials [materialIndex];

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}

	void OnPhotonPlayerInstantiate (PhotonPlayer player)
	{

	}
}