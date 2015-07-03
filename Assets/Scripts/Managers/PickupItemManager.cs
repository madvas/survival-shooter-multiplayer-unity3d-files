using UnityEngine;
using System.Collections.Generic;

public class PickupItemManager : MonoBehaviour
{

	public string prefabName;
	public int maxCount;
	public float respawnDelay;
	public float initialDelay;
	
	List<GameObject> itemInstances = new List<GameObject> ();
	
	void OnRoundStarted ()
	{
		if (PhotonNetwork.isMasterClient) {

			InvokeRepeating ("SpawnItem", initialDelay, respawnDelay);
		}
	}
	
	void OnPauseStarted ()
	{
		if (PhotonNetwork.isMasterClient) {
			CancelInvoke ("SpawnItem");
		}
	}
	
	void SpawnItem ()
	{
		Debug.Log ("spawning item");
		if (itemInstances.Count >= maxCount) {
			return;
		}
		PositionData randomTransform = PositionHelper.GetRandomSpawnPosition ();
		randomTransform.position.y = 1;
		itemInstances.Add (PhotonNetwork.Instantiate (prefabName, randomTransform.position, randomTransform.rotation, 0));
	}
}
