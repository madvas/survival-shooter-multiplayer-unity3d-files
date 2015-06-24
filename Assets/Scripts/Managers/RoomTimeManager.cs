using ExitGames.Client.Photon;
using UnityEngine;


/// <summary>
/// Simple script that uses a property to sync a start time for a multiplayer game.
/// </summary>
/// <remarks>
/// When entering a room, the first player will store the synchronized timestamp. 
/// You can't set the room's synchronized time in CreateRoom, because the clock on the Master Server
/// and those on the Game Servers are not in sync. We use many servers and each has it's own timer.
/// 
/// Everyone else will join the room and check the property to calculate how much time passed since start.
/// You can start a new round whenever you like.
/// 
/// Based on this, you should be able to implement a synchronized timer for turns between players.
/// </remarks>
public class RoomTimeManager : MonoBehaviour
{
	public int SecondsPerRound = 5;                  // time per round/turn
	public int SecondsPerPause = 10;
	public double StartTime;                        // this should could also be a private. i just like to see this in inspector
	
	private bool startRoundWhenTimeIsSynced;        // used in an edge-case when we wanted to set a start time but don't know it yet.
	private const string StartTimeKey = "st";       // the name of our "start time" custom property.
	private const string IsPauseKey = "p";

	public delegate void OnSecondElapsedAction (string remainingTime);
	public static event OnSecondElapsedAction onSecondElapsed;

	bool isPause = true;

	private void InitTimerNow ()
	{
		Debug.Log ("start round now");
		// in some cases, when you enter a room, the server time is not available immediately.
		// time should be 0.0f but to make sure we detect it correctly, check for a very low value.
		if (PhotonNetwork.time < 0.0001f) {
			// we can only start the round when the time is available. let's check that in Update()
			startRoundWhenTimeIsSynced = true;
			return;
		}
		startRoundWhenTimeIsSynced = false;
		SetNewStartTime ();
		InvokeRepeating ("OnSecondElapsed", 0, 1);
	}
	
	
	/// <summary>Called by PUN when this client entered a room (no matter if joined or created).</summary>
	public void OnJoinedRoom ()
	{
		Debug.Log ("timemanager joined room");
		if (PhotonNetwork.isMasterClient) {
			this.InitTimerNow ();
		} else {
			// as the creator of the room sets the start time after entering the room, we may enter a room that has no timer started yet
			Debug.Log ("StartTime already set: " + PhotonNetwork.room.customProperties.ContainsKey (StartTimeKey));
			InvokeRepeating ("OnSecondElapsed", 0, 1);
		}
	}
	
	/// <summary>Called by PUN when new properties for the room were set (by any client in the room).</summary>
	public void OnPhotonCustomRoomPropertiesChanged (Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey (StartTimeKey)) {
			StartTime = (double)propertiesThatChanged [StartTimeKey];
		}
		if (propertiesThatChanged.ContainsKey (IsPauseKey)) {
			isPause = (bool)propertiesThatChanged [IsPauseKey];
			if (isPause) {
				GameObjectHelper.SendMessageToAll ("OnPauseStarted");
			} else {
				GameObjectHelper.SendMessageToAll ("OnRoundStarted");
			}
		}
	}
	
	/// <remarks>
	/// In theory, the client which created the room might crash/close before it sets the start time.
	/// Just to make extremely sure this never happens, a new masterClient will check if it has to
	/// start a new round.
	/// </remarks>
	public void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		if (!PhotonNetwork.room.customProperties.ContainsKey (StartTimeKey)) {
			Debug.Log ("The new master starts a new round, cause we didn't start yet.");
			this.InitTimerNow ();
		}
	}

	public bool isPauseState ()
	{
		return isPause;
	}
	
	
	void Update ()
	{
		if (startRoundWhenTimeIsSynced) {
			this.InitTimerNow ();   // the "time is known" check is done inside the method.
		}
	}

	void OnSecondElapsed ()
	{
		int remainingSeconds = (int)Mathf.Round ((float)(SecondsPerRound - (PhotonNetwork.time - StartTime)));
		if (onSecondElapsed != null) {
			onSecondElapsed (TimeHelper.SecondsToTimer ((float)remainingSeconds));
		}

		if (PhotonNetwork.isMasterClient && remainingSeconds == 0f) {
			SetNewStartTime ();
		}
	}

	void SetNewStartTime ()
	{
		SetRoomProperty (StartTimeKey, PhotonNetwork.time);
		SetRoomProperty (IsPauseKey, !isPause);
	}

	void SetRoomProperty<T> (string name, T value)
	{
		ExitGames.Client.Photon.Hashtable prop = new Hashtable ();  // only use ExitGames.Client.Photon.Hashtable for Photon
		prop [name] = value;
		PhotonNetwork.room.SetCustomProperties (prop);   
	}
}