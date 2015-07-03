using UnityEngine;
using System.Collections;

public class GunBarrelEnd : MonoBehaviour
{
	float timer;           

	ParticleSystem gunParticles;                    
	LineRenderer gunLine;                           
	AudioSource gunAudio;                           
	Light gunLight;                                 
	float effectsDisplayTime = 0.2f;                


	void Awake ()
	{
		gunParticles = GetComponentInChildren<ParticleSystem> ();
		gunLine = GetComponentInChildren<LineRenderer> ();
		gunAudio = gunBarrelEnd.GetComponent<AudioSource> ();
		gunLight = GetComponentInChildren<Light> ();
		gunEndTransform = gunBarrelEnd.transform;
		playerHealth = GetComponent<PlayerHealth> ();
		anim = GetComponent<Animator> ();
	}
	
	void Update ()
	{
		timer += Time.deltaTime;
			
		if (timer >= timeBetweenBullets * effectsDisplayTime) {
			DisableEffects ();
		}
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

}
