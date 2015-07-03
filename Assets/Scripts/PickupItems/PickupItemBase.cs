using UnityEngine;
using System.Collections;

public class PickupItemBase : MonoBehaviour
{
	void OnTriggerEnter (Collider other)
	{
		PhotonView otherpv = other.GetComponent<PhotonView> ();
		if (otherpv != null && otherpv.isMine) {
			Debug.Log ("OnTriggerEnter() calls Pickup().");
			Pickup ();
		}
	}

	void Pickup ()
	{

	}
}
