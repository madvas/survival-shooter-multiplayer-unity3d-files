using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

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

		List<int> availableMaterials = Enumerable.Range (0, playerMaterials.Length).ToList ().Except (PhotonNetwork.playerList.GetMaterials ()).ToList ();
		int materialIndex = availableMaterials.PickRandom ();
		PhotonNetwork.player.SetMaterialIndex (materialIndex);

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}

	void OnPhotonPlayerPropertiesChanged (Hashtable playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.PlayerMaterialProp)) {
			int materialIndex = (int)props [PhotonPlayerExtensions.PlayerMaterialProp];
			SkinnedMeshRenderer body = player.FindComponentInChildWithTag<SkinnedMeshRenderer> ("PlayerBodyMesh");
			body.material = playerMaterials [materialIndex];
		}
	}
}