using UnityEngine;
using System.Collections;

public class DamagePickup : PickupItemBase
{
	public int increasedDamage = 15;
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		player.BroadcastMessageMultiArg ("OnPlayerDamageChange", increasedDamage, bonusDuration);
	}
}
