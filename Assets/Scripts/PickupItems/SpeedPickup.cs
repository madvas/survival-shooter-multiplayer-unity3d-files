using UnityEngine;
using System.Collections;

public class SpeedPickup : PickupItemBase
{
	public int increasedSpeed;
	public int bonusDuration;
	
	void OnActivateItemEffect (GameObject player)
	{
		player.BroadcastMessageMultiArg ("OnPlayerSpeedChange", increasedSpeed, bonusDuration);
	}
}

