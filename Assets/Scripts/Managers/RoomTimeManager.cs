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

	bool isPause = true;

	private void InitTimerNow ()
	{
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
	void OnJoinedRoom ()
	{
		if (PhotonNetwork.isMasterClient) {
			this.InitTimerNow ();
		} else {
			string methodName = isPause ? "OnJoinedRoomInPause" : "OnJoinedRoomInRound";
			GameObjectHelper.SendMessageToAll (methodName);
			InvokeRepeating ("OnSecondElapsed", 0, 1);
		}
	}

	void OnLeftRoom ()
	{
		CancelInvoke ("OnSecondElapsed");
		startRoundWhenTimeIsSynced = true;
		isPause = true;
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
	void OnMasterClientSwitched (PhotonPlayer newMasterClient)
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
		int totalTime = isPause ? SecondsPerPause : SecondsPerRound;
		int remainingSeconds = (int)Mathf.Round ((float)(totalTime - (PhotonNetwork.time - StartTime)));
		GameObjectHelper.SendMessageToAll ("OnTimerTick", TimeHelper.SecondsToTimer ((float)remainingSeconds));

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
		if (PhotonNetwork.inRoom) {
			ExitGames.Client.Photon.Hashtable prop = new Hashtable (); 
			prop [name] = value;
			PhotonNetwork.room.SetCustomProperties (prop);   
		}
	}
}