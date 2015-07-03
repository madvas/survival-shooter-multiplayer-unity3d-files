using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class PlayerShooting : MonoBehaviour
{
	public int damagePerShot;
	public float timeBetweenBullets = 0.15f;        // The time between each shot.
	public float range = 100f;                      // The distance the gun can fire.


	float timer;                                    // A timer to determine when to fire.
	Ray shootRay;                                   // A ray from the gun end forwards.
	RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
	int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
	PhotonView photonView;
	Transform playerTransform;
	int originalDamagePerShot;

	void Awake ()
	{
		shootableMask = LayerMask.GetMask ("Shootable");
		photonView = GetComponentInParent<PhotonView> ();
		originalDamagePerShot = damagePerShot;
	}

	void Update ()
	{
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

#if !MOBILE_INPUT
		// If the Fire1 button is being press and it's time to fire...
		if (Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
			// ... shoot the gun.
			Shoot ();
		}
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
	}

	void Shoot ()
	{
		timer = 0f;
		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;

		if (Physics.Raycast (shootRay, out shootHit, range, shootableMask)) {
			photonView.RPC ("Shoot", PhotonTargets.All, shootHit.point);
			if (shootHit.transform.tag == "Player") {
				shootHit.transform.GetComponent<PhotonView> ().RPC ("TakeShot", PhotonTargets.All, damagePerShot, PhotonNetwork.player.ID, shootHit.point);
			}
		} else {
			photonView.RPC ("Shoot", PhotonTargets.All, shootRay.origin + shootRay.direction * range);
		}
	}

	void OnPlayerDamageChange (object[] changeData)
	{
		Debug.Log ("OnPlayerDamageChange");
		if (!photonView.isMine) {
			return;
		}
		int damageBonus = (int)changeData [0];
		int bonusDuration = (int)changeData [1];

		damagePerShot += damageBonus;
		Debug.Log ("damage inreased to " + damagePerShot);
		Invoke ("ResetDamage", bonusDuration);
	}

	void ResetDamage ()
	{
		Debug.Log ("damage reset to " + originalDamagePerShot);
		damagePerShot = originalDamagePerShot;
	}

}