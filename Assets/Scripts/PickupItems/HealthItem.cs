using UnityEngine;
using System.Collections;

public class HealthItem : PickupItemBase
{
	public int healthBonus = 50;

	void OnActivateItemEffect (GameObject player)
	{
		player.SendMessage ("OnPlayerHealthChange", healthBonus);
	}
}
