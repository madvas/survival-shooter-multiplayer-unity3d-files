using UnityEngine;
using System.Collections;

public class DamagePickup : PickupItemBase
{
	public int increasedDamage = 15;
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		Debug.Log ("OnPlayerDamageChange");
		Debug.Log (player.GetComponent<PhotonView> ().owner.name);
		player.BroadcastMessageMultiArg ("OnPlayerDamageChange", increasedDamage, bonusDuration);
	}
}
