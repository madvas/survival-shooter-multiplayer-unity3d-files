using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMaterial : MonoBehaviour
{
	SkinnedMeshRenderer body;
	PlayerManager playerManager;

	void Awake ()
	{
		body = gameObject.FindComponentInChildWithTag<SkinnedMeshRenderer> ("PlayerBodyMesh");
		playerManager = GameObject.FindGameObjectWithTag ("PlayerManager").GetComponent<PlayerManager> ();
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.PlayerMaterialProp)) {
			int materialIndex = (int)props [PhotonPlayerExtensions.PlayerMaterialProp];
			body.material = playerManager.playerMaterials [materialIndex];
		}
	}
}
