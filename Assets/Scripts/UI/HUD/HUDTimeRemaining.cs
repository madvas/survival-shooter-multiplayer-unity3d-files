using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDTimeRemaining : MonoBehaviour
{

	Text timeText;
	
	void Awake ()
	{
		timeText = GetComponent<Text> ();
	}
	
	void OnTimerTick (object[] timeData)
	{
		string timeRemaining = timeData [0] as string;
		timeText.text = timeRemaining;
	}
}
