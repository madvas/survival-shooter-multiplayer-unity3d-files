using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

	void OnItemPicked (GameObject pickedItem)
	{
		if (PhotonNetwork.isMasterClient) {
			itemInstances.RemoveAll (item => item.GetInstanceID () == pickedItem.GetInstanceID ());
//			PhotonNetwork.Destroy (pickedItem);
		}

	}
	
	void SpawnItem ()
	{
		if (itemInstances.Count >= maxCount) {
			return;
		}
		PositionData randomTransform = PositionHelper.GetRandomSpawnPosition ();
		randomTransform.position.y = 1;
		itemInstances.Add (PhotonNetwork.Instantiate (prefabName, randomTransform.position, randomTransform.rotation, 0));
	}
}
