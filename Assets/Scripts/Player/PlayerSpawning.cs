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
	MeshRenderer meshRenderer;
	bool isSinking;

	void Awake ()
	{
		playerMovement = GetComponent <PlayerMovement> ();
		playerShooting = GetComponentInChildren <PlayerShooting> ();
		playerHealth = GetComponent<PlayerHealth> ();
		meshRenderer = GetComponent<MeshRenderer> ();
	}

	void Update ()
	{
		if (isSinking) {
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
			if (transform.position.y < -1) {
				isSinking = false;
				gameObject.SetActive (false);
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

	public void RespawnPlayer ()
	{
		GetComponent<CapsuleCollider> ().enabled = true;
		GetComponent <Rigidbody> ().isKinematic = false;
		if (photonView.isMine) {
			PositionData randomPosition = PositionManager.GetRandomSpawnPosition ();
			playerMovement.enabled = true;
			playerShooting.enabled = true;
			transform.position = randomPosition.position;
			transform.rotation = randomPosition.rotation;
			gameObject.SetActive (true);
		}
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}
	
	public void DestroyPlayer ()
	{
		if (photonView.isMine) {
			isSinking = true;
		}
		DisablePlayerControl ();
		GetComponent<CapsuleCollider> ().enabled = false;
		GetComponent <Rigidbody> ().isKinematic = true;
	}

	public void DisablePlayerControl ()
	{
		playerMovement.enabled = false;
		playerShooting.enabled = false;
	}

	void OnPlayerDead ()
	{
		Invoke ("RespawnPlayer", respawnDelay);
	}
}
