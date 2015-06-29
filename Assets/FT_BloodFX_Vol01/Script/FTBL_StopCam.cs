using UnityEngine;
using System.Collections;

public class FTBL_StopCam : MonoBehaviour {

	public bool myCheck = true;
	public GameObject camObject;
	Animator camAnim;

	void Start () {
		camAnim = camObject.GetComponent<Animator>();
	}

	void OnMouseDown () {
		if(myCheck == true){
			camAnim.speed = 0f;
			myCheck = false;
			return;
		}
		if(myCheck == false){
			camAnim.speed = 1f;
			myCheck = true;
			return;
		}
	}
}
