using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinNextRoundButton : MonoBehaviour
{

	Button button;
	Text text;
	Image image;
	bool clicked;

	void Awake ()
	{
		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
		image = GetComponent<Image> ();
	}

	void OnRoundStarted ()
	{
		SetButton (false);
	}

	void OnPauseStarted ()
	{
		clicked = false;
		SetButton (true);
	}

	public void OnClick ()
	{
		clicked = true;
		SetButton (false);
	}

	void SetButton (bool enabled)
	{
		button.enabled = enabled;
		text.enabled = enabled;
		image.enabled = enabled;
	}
}
