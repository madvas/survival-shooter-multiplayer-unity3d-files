using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ScoreboardTimeRemaining : MonoBehaviour
{

	Text timeText;

	void Awake ()
	{
		timeText = GetComponent<Text> ();
		RoomTimeManager.onSecondElapsed += OnSecondElapsed;
	}
	
	void OnSecondElapsed (string remainingTime)
	{
		timeText.text = remainingTime;
	}
}
