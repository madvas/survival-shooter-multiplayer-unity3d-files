using UnityEngine;
using System.Collections;

public class PlayerBody : Photon.MonoBehaviour
{
//	PlayerSpawning
//
//	void Awake ()
//	{
//		playerRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
//	}
//
//	void OnPlayerDamageChange (object[] changeData)
//	{
//		if (!photonView.isMine) {
//			return;
//		}
//		
//		photonView.owner.SetInvisibility (true);
//		CancelInvoke ("ResetVisibility");
//		Invoke ("ResetVisibility", bonusDuration);
//	}
//	
//	void ResetVisibility ()
//	{
//		Debug.Log ("damage reset to " + originalDamagePerShot);
//		damagePerShot = originalDamagePerShot;
//		photonView.owner.SetIncreasedDamage (false);
//	}
}
