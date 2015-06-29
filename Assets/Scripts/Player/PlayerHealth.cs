using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : Photon.MonoBehaviour
{
	public int startingHealth = 100;


	public Slider healthSlider = null;                                 
	public Image damageImage = null;                                   
	public AudioClip deathClip;
	public AudioClip hurtClip;
	public float flashSpeed = 5f;                               
	public Color flashColour = new Color (1f, 0f, 0f, 0.1f);    
	public Transform[] bloodEffects;

	Animator anim;                                              
	AudioSource playerAudio;                                    

	PlayerSpawning playerSpawning;
	bool isDead;                                                
	bool damaged;
	int dieAnimHash = Animator.StringToHash ("Die");
	bool dieAnimEnded = true;

	int _currentHealth;
	public int currentHealth {
		get {
			return _currentHealth;
		}
		set {
			_currentHealth = value;
			if (healthSlider) {
				healthSlider.value = value;
			}
		}
	}                                  

	void Awake ()
	{
		playerAudio = GetComponent <AudioSource> ();
		anim = GetComponent <Animator> ();
		playerSpawning = GetComponent<PlayerSpawning> ();
		if (photonView.isMine) {
			healthSlider = GameObject.FindWithTag ("HealthSlider").GetComponent<Slider> () as Slider;
			damageImage = GameObject.FindWithTag ("DamageImage").GetComponent<Image> () as Image;
		}
	}

	void Start ()
	{
		ResetHealth ();
	}

	void Update ()
	{
		if (damageImage) {
			if (!isDead) {
				if (damaged) {
					damageImage.color = flashColour;
				} else {
					damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
				}
			}
			damaged = false;
		}
	}


	public void TakeDamage (int amount, Vector3 hitPosition, out bool died)
	{
		died = false;
		currentHealth -= amount;
		if (currentHealth > 0 || !dieAnimEnded) {
			Instantiate (bloodEffects.PickRandom (), hitPosition, Quaternion.identity);
		}
		if (isDead) {
			return;
		}
		if (photonView.isMine) {
			damaged = true;
			playerAudio.clip = hurtClip;
			playerAudio.Play ();
			if (healthSlider) {
				healthSlider.value = currentHealth;
			}
		}
		Debug.Log (dieAnimEnded);

		if (currentHealth <= 0 && !isDead) {
			died = true;
			dieAnimEnded = false;
			Death ();
		}
	}

	public void ResetHealth ()
	{
		currentHealth = startingHealth;
	}

	void OnPlayerDieAnimEnd ()
	{
		Debug.Log ("OnPlayerDieAnimEnd");
		dieAnimEnded = true;
	}

	void Death ()
	{
		isDead = true;
		anim.SetTrigger (dieAnimHash);
		playerAudio.clip = deathClip;
		playerAudio.Play ();
		gameObject.BroadcastMessage ("OnPlayerDead", SendMessageOptions.DontRequireReceiver);
	}

	void OnPlayerRespawn ()
	{
		ResetHealth ();
		isDead = false;
	}
}