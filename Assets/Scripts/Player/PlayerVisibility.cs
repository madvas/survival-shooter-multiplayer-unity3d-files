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
		if (player.ID == PhotonNetwork.player.ID && !enabled) {
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
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (player.ID == photonView.owner.ID && props.ContainsKey (PhotonPlayerExtensions.invisibilityProp)) {
			Debug.Log ("setting " + player.name + " visibility " + !player.isInvisible ());
			SetPlayerVisibility (player, !player.isInvisible ());
		}
	}

}
