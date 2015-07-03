using UnityEngine;
using System.Collections;

public class PickupItemsManager : MonoBehaviour
{

	public string prefabName;
	public int maxCount;
	public float respawnDelay;
	public float initialDelay;
	
	GameObject[] itemInstances;
	
	void OnRoundStarted ()
	{
		InvokeRepeating ("SpawnItem", initialDelay, respawnDelay);
	}
	
	void OnPauseStarted ()
	{
		CancelInvoke ("SpawnItem");
	}
	
	void SpawnItem ()
	{
		if (itemInstances.Length >= maxCount) {
			return;
		}
		PositionData randomTransform = PositionHelper.GetRandomSpawnPosition ();
		PhotonNetwork.Instantiate (prefabName, randomTransform.position, randomTransform.rotation, 0);
	}
}
