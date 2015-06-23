using UnityEngine;
using System.Collections;

namespace Networking
{
	public class Room
	{
		
		public byte maxPlayers = 0;
		public string name;
		public int playerCount = 0;
		
		public Room (string newName, byte newMaxPlayers, int newPlayerCount)
		{
			maxPlayers = newMaxPlayers;
			name = newName;
			playerCount = newPlayerCount;
		}
		
		public Room (RoomInfo room)
		{
			Debug.Log ("creating room " + room.name);
			Room (room.name, room.maxPlayers, room.playerCount);
		}
	}
}

