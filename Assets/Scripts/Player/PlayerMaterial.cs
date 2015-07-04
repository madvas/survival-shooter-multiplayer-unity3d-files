using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMaterial : Photon.MonoBehaviour
{
	SkinnedMeshRenderer body;
	PlayerManager playerManager;

	void Awake ()
	{
		body = gameObject.FindComponentInChildWithTag<SkinnedMeshRenderer> ("PlayerBodyMesh");
		playerManager = GameObject.FindGameObjectWithTag ("PlayerManager").GetComponent<PlayerManager> ();
	}

	void OnPhotonInstantiate (PhotonMessageInfo	info)
	{
		UpdatePlayerMaterial (photonView.owner, false);
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (player.ID == photonView.owner.ID && props.ContainsKey (PhotonPlayerExtensions.materialProp)) {
			UpdatePlayerMaterial (player, false);
		}
	}

	public void UpdatePlayerMaterial (PhotonPlayer player, bool transparent)
	{
		int materialIndex = player.GetMaterialIndex ();
		if (materialIndex > -1) {
			Material[] materials = transparent ? playerManager.playerTransparentMaterials : playerManager.playerMaterials;
			body.material = materials [materialIndex];
		}
	}
}
