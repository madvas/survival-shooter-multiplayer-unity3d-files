using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
	public float respawnDelay = 2f;
	public Material[] playerMaterials;

	public Material invisibleMaterial;

	public Color playerTextColor0;
	public Color playerTextColor1;
	public Color playerTextColor2;
	public Color playerTextColor3;
	public Color playerTextColor4;
	public Color playerTextColor5;
	public Color playerTextColor6;

	public Color[] playerTextColors;

	void Awake ()
	{
		playerTextColors = new Color[7];
		playerTextColors [0] = playerTextColor0;
		playerTextColors [1] = playerTextColor1;
		playerTextColors [2] = playerTextColor2;
		playerTextColors [3] = playerTextColor3;
		playerTextColors [4] = playerTextColor4;
		playerTextColors [5] = playerTextColor5;
		playerTextColors [6] = playerTextColor6;
	}

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

	public Color GetPlayerTextColor (PhotonPlayer player)
	{
		return playerTextColors [player.GetMaterialIndex ()];
	}
	
	public string GetPlayerColoredName (PhotonPlayer player)
	{
		return player.name.Colorize (GetPlayerTextColor (player));
	}
}