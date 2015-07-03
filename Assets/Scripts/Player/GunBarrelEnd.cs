using UnityEngine;
using System.Collections;

public class GunBarrelEnd : MonoBehaviour
{
	public float timeBetweenBullets = 0.15f;   
	public float effectsDisplayTime = 0.2f;           
	float timer;           

	ParticleSystem gunParticles;                    
	LineRenderer gunLine;                           
	AudioSource gunAudio;                           
	Light gunLight;                                 
	float effectsDisplayTime = 0.2f;                


	void Awake ()
	{
		ResetShotEffects();
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

	void DisableEffects ()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
	}

	void ResetShotEffects () {
		GameObject normalEffects = transform.GetChild(0);
		gunParticles = normalEffects.GetComponent<ParticleSystem> ();
		gunLine = normalEffects.GetComponent<LineRenderer> ();
		gunAudio = normalEffects.GetComponent<AudioSource> ();
		gunLight = GetComponentInChildren<Light> ();
	}

//	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
//	{
//		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
//		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
//		if (player.ID == photonView.owner.ID && props.ContainsKey (PhotonPlayerExtensions.materialProp)) {
//			int materialIndex = (int)props [PhotonPlayerExtensions.materialProp];
//			body.material = playerManager.playerMaterials [materialIndex];
//		}
//	}

}
