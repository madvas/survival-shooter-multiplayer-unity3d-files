using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerVisibility : Photon.MonoBehaviour
{

	SkinnedMeshRenderer[] playerRenderers;
	PlayerMaterial playerMaterial;

	void Awake ()
	{
		playerRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
		playerMaterial = GetComponent<PlayerMaterial> ();
	}

	void OnPlayerGoInvisible (int duration)
	{
		Debug.Log ("OnPlayerGoInvisible");
		if (!photonView.isMine) {
			return;
		}
		
		photonView.owner.SetInvisibility (true);
		CancelInvoke ("ResetVisibility");
		Invoke ("ResetVisibility", duration);
	}
	
	void ResetVisibility ()
	{
		photonView.owner.SetInvisibility (false);
	}
	
	void OnPlayerRespawn ()
	{
		ResetVisibility ();
	}

	void OnPlayerDead ()
	{
		ResetVisibility ();
	}

	void SetPlayerVisibility (PhotonPlayer player, bool enabled)
	{
		Debug.Log ("SetPlayerVisibility " + player.name);
		if (player.ID == photonView.owner.ID && !enabled) {
			playerMaterial.UpdatePlayerMaterial (player, true);
		} else {
			playerMaterial.UpdatePlayerMaterial (player, false);
			foreach (var item in playerRenderers) {
				item.enabled = enabled;
			}
		}
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		Debug.Log ("invisibility OnPhotonPlayerPropertiesChanged");
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.invisibilityProp)) {
			SetPlayerVisibility (player, !player.isInvisible ());
		}
	}

}
