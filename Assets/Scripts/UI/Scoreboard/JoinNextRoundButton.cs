using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinNextRoundButton : MonoBehaviour
{

	Button button;
	Text text;

	void Awake ()
	{
		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
	}

	void OnRoundStarted ()
	{
		SetButton (false);
	}

	void OnPauseStarted ()
	{
		SetButton (true);
	}

	public void OnClick ()
	{
		SetButton (false);
	}

	void SetButton (bool enabled)
	{
		button.enabled = enabled;
		text.enabled = enabled;
	}
}
