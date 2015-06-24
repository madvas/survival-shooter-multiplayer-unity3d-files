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
	
	void OnTimerTick (string remainingTime)
	{
		timeText.text = remainingTime;
	}
}
