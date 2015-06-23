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
	
	Animator anim;                                              
	AudioSource playerAudio;                                    

	PlayerSpawning playerSpawning;
	bool isDead;                                                
	bool damaged;     

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

	void OnEnable ()
	{
		playerSpawning.onPlayerRespawn += onPlayerRespawn;
	}
	
	
	void OnDisable ()
	{
		//playerSpawning.onPlayerRespawn -= onPlayerRespawn;
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
		if (photonView.isMine && Input.GetKeyDown (KeyCode.Q)) {
			bool died;
			TakeDamage (100, out died);
		}
	}


	public void TakeDamage (int amount, out bool died)
	{
		died = false;
		if (isDead) {
			return;
		}
		currentHealth -= amount;
		if (photonView.isMine) {
			damaged = true;
			playerAudio.clip = hurtClip;
			playerAudio.Play ();
			if (healthSlider) {
				healthSlider.value = currentHealth;
			}
		}

		if (currentHealth <= 0 && !isDead) {
			died = true;
			Death ();
		}
	}

	public void ResetHealth ()
	{
		currentHealth = startingHealth;
	}

	void Death ()
	{
		playerSpawning.DisablePlayer (); 
		isDead = true;
		anim.SetTrigger ("Die");
		playerAudio.clip = deathClip;
		playerAudio.Play ();
	}

	void onPlayerRespawn ()
	{
		Debug.Log ("health respawn");
		ResetHealth ();
		isDead = false;
	}
}