using UnityEngine;
using System.Collections;

public class PlayerBody : Photon.MonoBehaviour
{

	SkinnedMeshRenderer[] playerRenderers;

	void Awake ()
	{
		playerRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
	}

	void OnPlayerGoInvisible (int duration)
	{
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

	void SetPlayerVisibility (bool enabled)
	{
		foreach (var item in playerRenderers) {
			item.enabled = enabled;
		}
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		Debug.Log ("invisibility OnPhotonPlayerPropertiesChanged");
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.invisibilityProp)) {
			SetPlayerVisibility (!player.isInvisible ());
		}
	}

}
