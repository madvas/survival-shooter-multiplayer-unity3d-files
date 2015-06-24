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

	void Awake ()
	{
		roomTimeManager = GameObject.FindGameObjectWithTag ("RoomTimeManager").GetComponent<RoomTimeManager> ();
		playerMovement = GetComponent <PlayerMovement> ();
		playerShooting = GetComponentInChildren <PlayerShooting> ();
		playerHealth = GetComponent<PlayerHealth> ();
		playerRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
	}

	void Update ()
	{
		if (isSinking) {
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
			if (transform.position.y < -1) {
				isSinking = false;
				SetPlayerVisibility (false);
			}
		}
	}

	void OnPlayerInstantiated ()
	{
		if (roomTimeManager.isPauseState ()) {
			DestroyPlayer (true);
		}
	}

	void OnPauseStarted ()
	{
		DestroyPlayer ();
	}

	void OnRoundStarted ()
	{
		RespawnPlayer ();
	}

	void RespawnPlayer ()
	{
		if (roomTimeManager.isPauseState ()) {
			return;
		}

		PositionData randomPosition = PositionHelper.GetRandomSpawnPosition ();
		transform.position = randomPosition.position;
		transform.rotation = randomPosition.rotation;
		SetPlayerPhysics (true);
		SetPlayerVisibility (true);
		SetPlayerControl (true);
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}

	// Called at the end of "Die" animation, must be public
	public void DestroyPlayer (bool instantly = false)
	{
		if (instantly) {
			SetPlayerVisibility (false);
		} else {
			isSinking = true;
		}

		SetPlayerPhysics (false);
		SetPlayerControl (false);
	}

	public void DestroyPlayerr ()
	{

	}

	void SetPlayerPhysics (bool enabled)
	{
		GetComponent<CapsuleCollider> ().enabled = enabled;
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
		playerMovement.enabled = enabled;
		playerShooting.enabled = enabled;
	}

	void OnPlayerDead ()
	{
		Invoke ("RespawnPlayer", respawnDelay);
	}
}
