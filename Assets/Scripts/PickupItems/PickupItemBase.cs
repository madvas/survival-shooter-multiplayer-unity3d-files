using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PickupItemBase : Photon.MonoBehaviour
{

	bool SentPickup;

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
		if (SentPickup) {
			return;
		}
		
		SentPickup = true;
		photonView.RPC ("PunPickup", PhotonTargets.AllViaServer);
	}


	[PunRPC]
	void PunPickup (PhotonMessageInfo msgInfo)
	{
		if (msgInfo.sender.isLocal) {
			SentPickup = false;
		}
		gameObject.SendMessage ("OnActivateItemEffect");
		PhotonNetwork.Destroy (gameObject);
	}
}
