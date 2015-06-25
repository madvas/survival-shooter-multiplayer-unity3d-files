using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinNextRoundButton : MonoBehaviour
{

	Button button;

	void Awake ()
	{
		button = GetComponent<Button> ();
	}

	void OnRoundStarted ()
	{
		button.enabled = false;
	}

	void OnPauseStarted ()
	{
		button.enabled = true;
	}

	public void OnClick ()
	{
		button.enabled = false;
	}
}
