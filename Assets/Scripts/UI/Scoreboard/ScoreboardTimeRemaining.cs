using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ScoreboardTimeRemaining : MonoBehaviour
{

	public string pausePrefix = "Next round starts in: ";
	public string roundPrefix = "Time remaining: ";

	Text timeText;
	string prefix;

	void Awake ()
	{
		timeText = GetComponent<Text> ();
	}

	void OnTimerTick (object[] timeData)
	{
		string remainingTime = timeData [0] as string;
		bool isPause = (bool)timeData [1];
		prefix = isPause ? pausePrefix : roundPrefix;
		timeText.text = prefix + remainingTime;
	}
}
