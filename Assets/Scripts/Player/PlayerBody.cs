using UnityEngine;
using System.Collections;

public class PlayerBody : Photon.MonoBehaviour
{
	void OnPlayerGoInvisible (int duration)
	{
		if (!photonView.isMine) {
			return;
		}
		int increasedDamage = (int)changeData [0];
		int bonusDuration = (int)changeData [1];
		
		
		photonView.owner.SetIncreasedDamage (true);
		CancelInvoke ("ResetVisibility");
		Invoke ("ResetVisibility", bonusDuration);
	}
	
	void ResetVisibility ()
	{
		photonView.owner.SetInvisibility (false);
	}
	
	void OnPlayerRespawn ()
	{
		ResetVisibility ();
	}
}
