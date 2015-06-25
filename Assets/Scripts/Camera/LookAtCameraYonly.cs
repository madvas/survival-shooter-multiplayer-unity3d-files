using UnityEngine;
using System.Collections;

public class LookAtCameraYonly : MonoBehaviour
{
	public Camera cameraToLookAt;
	
	void Update ()
	{
		Vector3 v = cameraToLookAt.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt (cameraToLookAt.transform.position - v); 
	}
}