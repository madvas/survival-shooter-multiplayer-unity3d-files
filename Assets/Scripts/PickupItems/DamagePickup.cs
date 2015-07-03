using UnityEngine;
using System.Collections;

public class DamagePickup : PickupItemBase
{
	public int damageBonus = 5;
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		Debug.Log ("damage activate");
		player.SendMessageMultiArg ("OnPlayerDamageChange", damageBonus, bonusDuration);
	}
}
