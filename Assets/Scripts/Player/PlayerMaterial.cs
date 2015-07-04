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
		int materialIndex = photonView.owner.GetMaterialIndex ();
		if (materialIndex > -1) {
			body.material = playerManager.playerMaterials [materialIndex];
		}
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (player.ID == photonView.owner.ID && props.ContainsKey (PhotonPlayerExtensions.materialProp)) {
			int materialIndex = (int)props [PhotonPlayerExtensions.materialProp];
			body.material = playerManager.playerMaterials [materialIndex];
		}
	}

	public ChangePlayerMaterial(PhotonPlayer player, bool transparent) {
		Material[] materials = transparent ? playerManager.playerTransparentMaterials : playerManager.playerMaterials;
		body.material = materials[player.GetMaterialIndex()];
	}
}
