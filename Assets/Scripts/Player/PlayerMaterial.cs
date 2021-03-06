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
		UpdatePlayerMaterial (false);
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (player.ID == photonView.owner.ID && props.ContainsKey (PhotonPlayerExtensions.materialProp)) {
			UpdatePlayerMaterial (false);
		}
	}

	public void UpdatePlayerMaterial (bool transparent)
	{
		int materialIndex = photonView.owner.GetMaterialIndex ();
		if (materialIndex > -1) {
			Material[] materials = transparent ? playerManager.playerTransparentMaterials : playerManager.playerMaterials;
			body.material = materials [materialIndex];
		}
	}
}
