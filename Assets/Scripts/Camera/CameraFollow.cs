using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public float smoothing = 5f;        
	public Vector3 offset = new Vector3 (1, 15, -22);

	Transform target = null;            

	public void setTarget (Transform targetToFollow)
	{
		target = targetToFollow;
	}


	void FixedUpdate ()
	{
		if (!target) {
			return;
		}

		Vector3 targetCamPos = target.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}		
}