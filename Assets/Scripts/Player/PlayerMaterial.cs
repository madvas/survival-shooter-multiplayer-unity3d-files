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
		Debug.Log ("OnPhotonInstantiate");
		int materialIndex = photonView.owner.GetMaterialIndex ();
		if (materialIndex > -1) {
			body.material = playerManager.playerMaterials [materialIndex];
			Debug.Log ("setting trans");
			Color col = body.material.color;
			col.a = 0.1f;
			body.material.color = col;
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
}
