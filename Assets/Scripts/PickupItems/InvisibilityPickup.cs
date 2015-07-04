using UnityEngine;
using System.Collections;

public class InvisibilityPickup : PickupItemBase
{
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		player.BroadcastMessage ("OnPlayerGoInvisible", bonusDuration);
	}
}
