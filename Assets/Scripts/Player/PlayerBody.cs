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
		SetPlayerVisibility (true);
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
}
