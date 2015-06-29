using UnityEngine;
using System.Collections;

public class PlayerNetworking : Photon.MonoBehaviour
{
	public float timeBetweenBullets = 0.15f;   

	float timer;                   
	bool initialLoad = true;
	Vector3 position;
	Quaternion rotation;
	float smoothing = 10f;
	Animator anim;
	bool isWalking = false;

	ParticleSystem gunParticles;                    // Reference to the particle system.
	LineRenderer gunLine;                           // Reference to the line renderer.
	AudioSource gunAudio;                           // Reference to the audio source.
	Light gunLight;                                 // Reference to the light component.
	float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
	Transform gunEndTransform;
	PlayerHealth playerHealth;
	PlayerSpawning playerSpawning;
	int IsWalkingHash = Animator.StringToHash ("IsWalking");
	int respawnHash = Animator.StringToHash ("Respawn");

	void Awake ()
	{
		GameObject gunBarrelEnd = transform.Find ("GunBarrelEnd").gameObject;
		gunParticles = GetComponentInChildren<ParticleSystem> ();
		gunLine = GetComponentInChildren<LineRenderer> ();
		gunAudio = gunBarrelEnd.GetComponent<AudioSource> ();
		gunLight = GetComponentInChildren<Light> ();
		gunEndTransform = gunBarrelEnd.transform;
		playerHealth = GetComponent<PlayerHealth> ();
		playerSpawning = GetComponent<PlayerSpawning> ();
		anim = GetComponent<Animator> ();
	}

	void Start ()
	{
		if (!photonView.isMine) {
			StartCoroutine (UpdateData ());
		} else {
			ResetScore ();
		}
	}

	void Update ()
	{
		timer += Time.deltaTime;
		
		if (timer >= timeBetweenBullets * effectsDisplayTime) {
			DisableEffects ();
		}
	}
	
	IEnumerator UpdateData ()
	{
		if (initialLoad) {
			initialLoad = false;
			transform.position = position;
			transform.rotation = rotation;
		}
		
		while (true) {
			transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * smoothing);
			anim.SetBool ("IsWalking", isWalking);
			yield return null;
		}
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			stream.SendNext (anim.GetBool (IsWalkingHash));
		} else {
			position = (Vector3)stream.ReceiveNext ();
			rotation = (Quaternion)stream.ReceiveNext ();
			isWalking = (bool)stream.ReceiveNext ();
		}
	}

	[PunRPC]
	public void Shoot (Vector3 hitPositon)
	{
		DrawShot (gunEndTransform.position, hitPositon);
	}

	[PunRPC]
	public void TakeShot (int damage, int shooterId, Vector3 hitPositon)
	{
		bool died;
		playerHealth.TakeDamage (damage, hitPositon, out died);
		if (died) {
			if (photonView.isMine) {
				photonView.owner.AddDeaths (1);
			}

			if (shooterId == PhotonNetwork.player.ID) {
				PhotonNetwork.player.AddScore (1);
			}
			PhotonPlayer killer = PhotonPlayer.Find (shooterId);
			GameObjectHelper.SendMessageToAll ("OnPlayerKill", killer, photonView.owner);
		}
	}

	[PunRPC]
	public void RespawnPlayer ()
	{
		anim.SetTrigger (respawnHash);
	}

	public void DrawShot (Vector3 fromPosition, Vector3 hitPositon)
	{
		timer = 0f;
		gunAudio.Play ();
		gunLight.enabled = true;
		gunParticles.Stop ();
		gunParticles.Play ();
		gunLine.enabled = true;
		gunLine.SetPosition (0, fromPosition);
		gunLine.SetPosition (1, hitPositon);
	}

	void OnPlayerRespawn ()
	{
		if (photonView.isMine) {
			photonView.RPC ("RespawnPlayer", PhotonTargets.All);
		}
	}

	void OnRoundStarted ()
	{
		ResetScore ();
	}

	void ResetScore ()
	{
		if (photonView.isMine) {
			photonView.owner.SetScore (0);
			photonView.owner.SetDeaths (0);
		}
	}

	void DisableEffects ()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
	}
}
