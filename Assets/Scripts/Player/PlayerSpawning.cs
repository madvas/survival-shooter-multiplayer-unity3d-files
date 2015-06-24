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

	void Awake ()
	{
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
		Debug.Log ("Respawning playering");
		GetComponent<CapsuleCollider> ().enabled = true;
		GetComponent <Rigidbody> ().isKinematic = false;
		if (photonView.isMine) {
			PositionData randomPosition = PositionHelper.GetRandomSpawnPosition ();
			playerMovement.enabled = true;
			playerShooting.enabled = true;
			transform.position = randomPosition.position;
			transform.rotation = randomPosition.rotation;
		}
		SetPlayerVisibility (true);
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}
	
	void DestroyPlayer ()
	{
		if (photonView.isMine) {
			isSinking = true;
		}
		DisablePlayerControl ();
		GetComponent<CapsuleCollider> ().enabled = false;
		GetComponent <Rigidbody> ().isKinematic = true;
	}

	void SetPlayerVisibility (bool enabled)
	{
		foreach (var item in playerRenderers) {
			item.enabled = enabled;
		}
	}

	void DisablePlayerControl ()
	{
		playerMovement.enabled = false;
		playerShooting.enabled = false;
	}


	void OnPlayerDead ()
	{
		Invoke ("RespawnPlayer", respawnDelay);
	}
}
