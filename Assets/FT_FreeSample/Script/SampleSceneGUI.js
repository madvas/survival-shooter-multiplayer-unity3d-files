var mySkin : GUISkin;
var effect01 : GameObject;
var effect02 : GameObject;
var effect03 : GameObject;
var effect04 : GameObject;
var effect05 : GameObject;
var effect06 : GameObject;
var effect07 : GameObject;


function OnGUI ()
{
	GUI.skin = mySkin;
	
	GUI.Label (Rect (70,10,100,20), "FT FreeSample DEMO");

	if(GUI.Button (Rect (10,40,20,20), GUIContent ("", "FT CartoonEffect Volume02 --- DarkCircle01_5sec")))
	{	Instantiate(effect01, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (40,40,20,20), GUIContent ("", "FT MagicEffect Volume03 --- GroundFX_Fire01")))
	{	Instantiate(effect02, new Vector3(0, 0.1, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (70,40,20,20), GUIContent ("", "FT ImpactEffect Volume01 --- HitEffect01")))
	{	Instantiate(effect03, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (100,40,20,20), GUIContent ("", "FT CartoonEffect Volume01 --- Landing01_1shot")))
	{	Instantiate(effect04, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (130,40,20,20), GUIContent ("", "FT Pulse Volume01 --- RedFlareFX_5sec")))
	{	Instantiate(effect05, new Vector3(0, 1.5, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (160,40,20,20), GUIContent ("", "FT Slasher --- Slash09_Mesh")))
	{	Instantiate(effect06, new Vector3(0, 1.0, 0), Quaternion.Euler(0, 0, 0));	}
	if(GUI.Button (Rect (10,70,20,20), GUIContent ("", "FT FreeSample --- RocketFire01")))
	{	Instantiate(effect07, new Vector3(0, 1.0, 0), Quaternion.Euler(0, 0, 0));	}
	
	GUI.Label (Rect (10,Screen.height-30,300,25), GUI.tooltip);
}