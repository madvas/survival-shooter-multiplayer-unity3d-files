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
	Animator anim;          
	bool isSinking;

	// Use this for initialization
	void Awake ()
	{
		playerMovement = GetComponent <PlayerMovement> ();
		playerShooting = GetComponentInChildren <PlayerShooting> ();
		playerHealth = GetComponent<PlayerHealth> ();
		anim = GetComponent <Animator> ();
	}

	void Update ()
	{
		if (isSinking) {
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
			if (transform.position.y < -1) {
				isSinking = false;
			}
		}
	}

	public void RespawnPlayer ()
	{
		Debug.Log ("RespawnPlayer");
		Debug.Log (photonView.isMine);
		GetComponent<CapsuleCollider> ().enabled = true;
		GetComponent <Rigidbody> ().isKinematic = false;
		if (photonView.isMine) {
			PositionData randomPosition = PositionManager.GetRandomSpawnPosition ();
			playerMovement.enabled = true;
			playerShooting.enabled = true;
			transform.position = randomPosition.position;
			transform.rotation = randomPosition.rotation;
		}
		gameObject.BroadcastMessage ("OnPlayerRespawn", SendMessageOptions.DontRequireReceiver);
	}
	
	public void DestroyPlayer ()
	{
		if (photonView.isMine) {
			isSinking = true;
		}
		GetComponent<CapsuleCollider> ().enabled = false;
		GetComponent <Rigidbody> ().isKinematic = true;
	}

	public void DisablePlayer ()
	{
		playerMovement.enabled = false;
		playerShooting.enabled = false;
	}

	void OnPlayerDead ()
	{
		Invoke ("RespawnPlayer", respawnDelay);
	}
}
