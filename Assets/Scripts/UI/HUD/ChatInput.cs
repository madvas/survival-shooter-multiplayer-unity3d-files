using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ChatInput : MonoBehaviour
{

	InputField messageInput;

	void Awake ()
	{
		messageInput = GetComponentInChildren<InputField> ();
	}

//	void Update ()
//	{
//		if (Input.GetKeyDown (KeyCode.Return)) {
//			if (messageInput.isFocused) {
//
//			}
//			messageInput.ActivateInputField ();
//		}
//	}
}
