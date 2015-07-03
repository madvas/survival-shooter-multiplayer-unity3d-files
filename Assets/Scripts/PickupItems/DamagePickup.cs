using UnityEngine;
using System.Collections;

public class HealthPickup : PickupItemBase
{
	public int damageBonus = 5;
	
	void OnActivateItemEffect (GameObject player)
	{
		Debug.Log ("OnActivateItemEffect");
		player.SendMessage ("OnPlayerHealthChange", healthBonus);
	}
}
