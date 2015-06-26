using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JoinNextRoundButton : MonoBehaviour
{

	Button button;
	Text text;
	Image image;
	bool clicked;
	NetworkManager networkManager;

	void Awake ()
	{
		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
		image = GetComponent<Image> ();
		networkManager = GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<NetworkManager> ();
	}

	void OnJoinedRoom ()
	{
		Debug.Log ("OnJoinedRoom");
		clicked = true;
		SetButton (false);
	}

	void OnRoundStarted ()
	{
		SetButton (false);
		if (!clicked) {
			networkManager.LeaveRoom ();
		}
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
