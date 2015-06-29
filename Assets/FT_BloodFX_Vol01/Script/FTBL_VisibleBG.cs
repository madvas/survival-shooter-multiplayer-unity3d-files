using UnityEngine;
using System.Collections;

public class FTBL_VisibleBG : MonoBehaviour {

	public bool myCheck = true;
	public GameObject BG;

	void Start () {

	}
	
	void OnMouseDown () {
		if(myCheck == true){
			BG.SetActive(false); 
			myCheck = false;
			return;
		}
		if(myCheck == false){
			BG.SetActive(true);
			myCheck = true;
			return;
		}
	}
}
