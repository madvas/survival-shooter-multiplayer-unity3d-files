using UnityEngine;
using System.Collections;

public class JoinNextRoundButton : MonoBehaviour {

	void OnRoundStarted() {
		gameObject.SetActive(false);
	}

	void OnPauseStarted() {
		gameObject.SetActive(true);
	}

	void public OnClick() {

	}
}
