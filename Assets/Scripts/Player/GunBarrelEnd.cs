using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
		SetShotEffects(normalEffects);
	}

	void ResetShotEffects () {
		GameObject enhancedEffects = transform.GetChild(1);
		SetShotEffects(enhancedEffects);
	}

	void SetShotEffects(GameObject effectsObject) {
		gunParticles = effectsObject.GetComponent<ParticleSystem> ();
		gunLine = effectsObject.GetComponent<LineRenderer> ();
		gunAudio = effectsObject.GetComponent<AudioSource> ();
		gunLight = effectsObject.GetComponent<Light> ();
	}

	void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
	{
		PhotonPlayer player = playerAndUpdatedProps [0] as PhotonPlayer;
		Hashtable props = playerAndUpdatedProps [1] as Hashtable;
		if (props.ContainsKey (PhotonPlayerExtensions.materialProp)) {
			int materialIndex = (int)props [PhotonPlayerExtensions.materialProp];
			body.material = playerManager.playerMaterials [materialIndex];
		}
	}

}
