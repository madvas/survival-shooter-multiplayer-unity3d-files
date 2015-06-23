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

	void Awake ()
	{
		Debug.Log ("network awake");
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
		Debug.Log ("network start");
		Debug.Log (photonView.isMine);
		playerSpawning.onPlayerRespawn += onPlayerRespawn;
		if (photonView.isMine) {
			GetComponentInChildren<PlayerShooting> ().enabled = true;
			GetComponent<PlayerMovement> ().enabled = true;
			GetComponent<PlayerSpawning> ().enabled = true;
			Debug.Log ("here ok");
			Debug.Log (transform.position);
		} else {
			StartCoroutine (UpdateData ());
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
			stream.SendNext (anim.GetBool ("IsWalking"));
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
	public void TakeShot (int damage, string shooterName)
	{
		playerHealth.TakeDamage (damage);
		if (photonView.isMine) {
			Debug.Log (PhotonNetwork.player.name + " was shot by " + shooterName);

		}
	}

	[PunRPC]
	public void RespawnPlayer ()
	{
		Debug.Log ("PlayerNetworking Respawn");
		anim.SetTrigger ("Respawn");
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

	void onPlayerRespawn ()
	{
		Debug.Log ("PlayerNetworking respawn");
		if (photonView.isMine) {
			photonView.RPC ("RespawnPlayer", PhotonTargets.All);
		}
	}

	void DisableEffects ()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
	}
}
