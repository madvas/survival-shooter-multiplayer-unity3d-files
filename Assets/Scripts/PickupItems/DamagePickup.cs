using UnityEngine;
using System.Collections;

public class DamagePickup : PickupItemBase
{
	public int damageBonus = 5;
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		Debug.Log ("OnActivateItemEffect");
		object[] damageData = new object[2];
		damageData [0] = damageBonus;
		damageData [1] = bonusDuration;
		player.SendMessage ("OnPlayerDamageChange", damageData);
	}
}
