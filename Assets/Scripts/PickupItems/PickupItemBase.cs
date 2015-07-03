using UnityEngine;
using System.Collections;

public class PickupItemBase : MonoBehaviour
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

	}

	public void Pickup ()
	{
		if (this.SentPickup) {
			// skip sending more pickups until the original pickup-RPC got back to this client
			return;
		}
		
		this.SentPickup = true;
		this.photonView.RPC ("PunPickup", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	public void PunPickup (PhotonMessageInfo msgInfo)
	{
		// when this client's RPC gets executed, this client no longer waits for a sent pickup and can try again
		if (msgInfo.sender.isLocal)
			this.SentPickup = false;
		
		
		// In this solution, picked up items are disabled. They can't be picked up again this way, etc.
		// You could check "active" first, if you're not interested in failed pickup-attempts.
		if (!this.gameObject.GetActive ()) {
			// optional logging:
			Debug.Log ("Ignored PU RPC, cause item is inactive. " + this.gameObject + " SecondsBeforeRespawn: " + SecondsBeforeRespawn + " TimeOfRespawn: " + this.TimeOfRespawn + " respawn in future: " + (TimeOfRespawn > PhotonNetwork.time));
			return;     // makes this RPC being ignored
		}
		
		
		// if the RPC isn't ignored by now, this is a successful pickup. this might be "my" pickup and we should do a callback
		this.PickupIsMine = msgInfo.sender.isLocal;
		
		// call the method OnPickedUp(PickupItem item) if a GameObject was defined as callback target
		if (this.OnPickedUpCall != null) {
			// you could also skip callbacks for items that are not picked up by this client by using: if (this.PickupIsMine)
			this.OnPickedUpCall.SendMessage ("OnPickedUp", this);
		}
		
		
		// setup a respawn (or none, if the item has to be dropped)
		if (SecondsBeforeRespawn <= 0) {
			this.PickedUp (0.0f);    // item doesn't auto-respawn. must be dropped
		} else {
			// how long it is until this item respanws, depends on the pickup time and the respawn time
			double timeSinceRpcCall = (PhotonNetwork.time - msgInfo.timestamp);
			double timeUntilRespawn = SecondsBeforeRespawn - timeSinceRpcCall;
			
			//Debug.Log("msg timestamp: " + msgInfo.timestamp + " time until respawn: " + timeUntilRespawn);
			
			if (timeUntilRespawn > 0) {
				this.PickedUp ((float)timeUntilRespawn);
			}
		}
	}
}
