using UnityEngine;
using System.Collections;

public class SpeedPickup : PickupItemBase
{
	public float increasedSpeed;
	public int bonusDuration;
	
	void OnActivateItemEffect (GameObject player)
	{
		player.BroadcastMessageMultiArg ("OnPlayerSpeedChange", increasedSpeed, bonusDuration);
	}
}

