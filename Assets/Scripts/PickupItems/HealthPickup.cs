using UnityEngine;
using System.Collections;

public class HealthPickup : PickupItemBase
{
	public int healthBonus = 50;

	void OnActivateItemEffect (GameObject player)
	{
		Debug.Log ("OnActivateItemEffect");
		player.SendMessage ("OnPlayerHealthChange", healthBonus);
	}
}
