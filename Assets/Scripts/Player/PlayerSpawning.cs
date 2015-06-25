using UnityEngine;
using System.Collections;

public class PlayerSpawning : Photon.MonoBehaviour
{
	public float sinkSpeed = 1;
	public float respawnDelay = 5;

	PlayerMovement playerMovement;                              
	PlayerShooting playerShooting;
	PlayerHealth playerHealth;
	SkinnedMeshRenderer[] playerRenderers;
	bool isSinking;
	RoomTimeManager roomTimeManager;
	int dieAnimHash = Animator.StringToHash ("Die");
	Animator anim;

	void Awake ()
	{
		roomTimeManager = GameObject.FindGameObjectWithTag ("RoomTimeManager").GetComponent<RoomTimeManager> ();
		playerMovement = GetComponent <PlayerMovement> ();
		playerShooting = GetComponentInChildren <PlayerShooting> ();
		playerHealth = GetComponent<PlayerHealth> ();
		playerRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
		anim = GetComponent<Animator> ();
	}

	void Update ()
	{
		if (photonView.isMine && isSinking) {
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
			if (transform.position.y < -1) {
				isSinking = false;
				SetPlayerVisibility (false);
			}
		}
	}

	void OnPhotonInstantiate (PhotonMessageInfo	info)
	{
		if (roomTimeManager.isPauseState ()) {
			DestroyPlayer (true);
		} else {
			RespawnPlayer ();
		}
	}

	void OnPauseStarted ()
	{
		DestroyPlayer ();
	}

	void OnRoundStarted ()
	{
		Debug.Log ("round started respawning");
		RespawnPlayer ();
	}

	void RespawnPlayer ()
	{
		Debug.Log ("respawn player");
		if (roomTimeManager.isPauseState ()) {
			return;
		}

		if (photonView.isMine) {
			PositionHelper.RandomizeTransform (transform);
			SetPlayerControl (true);
		}
		SetPlayerPhysics (true);
		SetPlayerVisibility (true);
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}

	void DestroyPlayer (bool instantly = false)
	{
		Debug.Log ("Destroying player: " + instantly);
		if (instantly) {
			SetPlayerVisibility (false);
		} else if (photonView.isMine) {
			isSinking = true;
		}

		SetPlayerPhysics (false);
		SetPlayerControl (false);
	}

	// Called at the end of "Die" animation, must be public
	public void OnDeathAnimEnd ()
	{
		Debug.Log ("Death anim ended");
		DestroyPlayer ();
	}

	void SetPlayerPhysics (bool enabled)
	{
		GetComponent<CapsuleCollider> ().enabled = enabled;
		GetComponent<BoxCollider> ().enabled = enabled;
		GetComponent <Rigidbody> ().isKinematic = !enabled;
	}

	void SetPlayerVisibility (bool enabled)
	{
		foreach (var item in playerRenderers) {
			item.enabled = enabled;
		}
	}

	void SetPlayerControl (bool enabled)
	{
		Debug.Log ("setting player control " + enabled);
		playerMovement.enabled = enabled;
		playerShooting.enabled = enabled;
	}

	void OnPlayerDead ()
	{
		SetPlayerControl (false);
		Invoke ("RespawnPlayer", respawnDelay);
	}
}
