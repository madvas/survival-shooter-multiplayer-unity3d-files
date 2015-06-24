using UnityEngine;
using System.Collections;

public class PlayerSpawning : Photon.MonoBehaviour
{

	public delegate void RespawnAction ();
	public event RespawnAction onPlayerRespawn;

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
		roomTimeManager = GameObject.FindGameObjectWithTag ("RoomTimeManager");
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
		Debug.Log ("spawn cauht OnPlayerInstantiated");
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
		if (photonView.isMine) {
			PositionData randomPosition = PositionHelper.GetRandomSpawnPosition ();
			playerMovement.enabled = true;
			playerShooting.enabled = true;
			transform.position = randomPosition.position;
			transform.rotation = randomPosition.rotation;
		}
		SetPlayerPhysics (true);
		SetPlayerVisibility (true);
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}
	
	void DestroyPlayer (bool instantly = false)
	{
		if (photonView.isMine && !instantly) {
			isSinking = true;
		}
		SetPlayerPhysics (false);
		SetPlayerControl (false);
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
