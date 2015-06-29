using UnityEngine;
using System.Collections;

public class FTBL_SampleSceneGUI : MonoBehaviour {

	public GUIText prefabName;
	public GameObject[] particlePrefab;
	public int particleNum = 0;

	GameObject effectPrefab;

	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButtonDown(0) ){ 
			if(particleNum == 0){
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 1000.0f)){				
					effectPrefab = (GameObject)Instantiate(particlePrefab[particleNum],
					new Vector3(hit.point.x,hit.point.y,hit.point.z), Quaternion.Euler(0,0,0));
					return;
				}
			}
			if(particleNum < 2){
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 1000.0f)){				
					effectPrefab = (GameObject)Instantiate(particlePrefab[particleNum],
					new Vector3(hit.point.x,hit.point.y + 2.1f,hit.point.z), Quaternion.Euler(0,0,0));
				}
			}
			else{
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 1000.0f)){				
					effectPrefab = (GameObject)Instantiate(particlePrefab[particleNum],
					                                       new Vector3(hit.point.x,hit.point.y,hit.point.z), Quaternion.Euler(0,0,0));
				}
			}
		}
		
		if (Input.GetKeyDown(KeyCode.LeftArrow)){
			Destroy(effectPrefab);
			particleNum -= 1;
			if( particleNum < 0) {
				particleNum = particlePrefab.Length-1;
			}	
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)){
			Destroy(effectPrefab);
			particleNum += 1;
			if(particleNum >(particlePrefab.Length - 1)) {
				particleNum = 0;
			}
		}
		
		prefabName.text= particlePrefab[particleNum].name;
		
	}
}
