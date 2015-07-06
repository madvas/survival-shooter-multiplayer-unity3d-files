using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PickupItemManager : MonoBehaviour
{

	public string prefabName;
	public int maxCount;
	public float respawnDelay;
	public float initialDelay;

	AudioSource audioSource;
	List<GameObject> itemInstances = new List<GameObject> ();

	void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
	}

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

	void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("The new master");
			InvokeRepeating ("SpawnItem", initialDelay, respawnDelay);
		}
	}

	void OnItemPicked (object[] pickData)
	{
		GameObject pickedItem = pickData [0] as GameObject;
		bool pickedByMe = (bool)pickData [1];
		if (pickedByMe) {
			audioSource.Play ();
		}
		if (PhotonNetwork.isMasterClient) {
			itemInstances.RemoveAll (item => item.GetInstanceID () == pickedItem.GetInstanceID ());
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
