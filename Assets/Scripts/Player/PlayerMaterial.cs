using UnityEngine;
using System.Collections;
using ExitGames.Client;

public class PlayerMaterial : MonoBehaviour
{

	void OnPhotonPlayerPropertiesChanged (Hashtable playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.PlayerMaterialProp)) {
			int materialIndex = (int)props [PhotonPlayerExtensions.PlayerMaterialProp];
			player.
				SkinnedMeshRenderer body = player.FindComponentInChildWithTag<SkinnedMeshRenderer> ("PlayerBodyMesh");
			body.material = playerMaterials [materialIndex];
		}
	}
}
