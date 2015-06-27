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

		List<int> availableMaterials = Enumerable.Range (0, playerMaterials.Length).ToList ().Except (PhotonNetwork.playerList.GetMaterials ()).ToList ();
		int materialIndex = availableMaterials.PickRandom ();
		PhotonNetwork.player.SetMaterialIndex (materialIndex);

		player.GetComponent<AudioListener> ().enabled = true;
		GameObjectHelper.SendMessageToAll ("OnMinePlayerInstantiate", player);
	}

	void SetMaterialOnOtherPlayers ()
	{
		foreach (var player in PhotonNetwork.playerList.GetOtherPlayers()) {
			int materialIndex = player.GetMaterialIndex ();

		}
	}
}