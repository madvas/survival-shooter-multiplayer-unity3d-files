using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

	Transform playerTransform = null;

	public float smoothing = 5f;        
	public Vector3 offset = new Vector3 (1, 15, -22);
	
	void FixedUpdate ()
	{
		if (!playerTransform) {
			return;
		}
		
		Vector3 targetCamPos = playerTransform.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}		

	void Awake ()
	{
		PlayerManager.onPlayerInstantiated += onPlayerInstantiated;
	}

	void onPlayerInstantiated (GameObject player)
	{
		playerTransform = player.transform;
	}
}
