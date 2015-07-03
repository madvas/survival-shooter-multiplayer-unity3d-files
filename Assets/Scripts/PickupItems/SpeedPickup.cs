using UnityEngine;
using System.Collections;

public class SpeedPickup : PickupItemBase
{
	public int increasedSpeed = 15;
	public int bonusDuration = 20;
	
	void OnActivateItemEffect (GameObject player)
	{
		player.BroadcastMessageMultiArg ("OnPlayerSpeedChange", increasedSpeed, bonusDuration);
	}
}

