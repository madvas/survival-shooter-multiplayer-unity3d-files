using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PunPlayerDeaths : MonoBehaviour
{
	public const string PlayerDeathsProp = "d";
}


static class DeathsExtensions
{
	public static void SetDeaths (this PhotonPlayer player, int newDeaths)
	{
		Hashtable deaths = new Hashtable ();  // using PUN's implementation of Hashtable
		deaths [PunPlayerDeaths.PlayerDeathsProp] = newDeaths;

		player.SetCustomProperties (deaths);  // this locally sets the deaths and will sync it in-game asap.
	}

	public static void AddDeaths (this PhotonPlayer player, int deathsToAddToCurrent)
	{
		int current = player.GetDeaths ();
		current = current + deathsToAddToCurrent;

		Hashtable deaths = new Hashtable ();  // using PUN's implementation of Hashtable
		deaths [PunPlayerDeaths.PlayerDeathsProp] = current;

		player.SetCustomProperties (deaths);  // this locally sets the deaths and will sync it in-game asap.
	}

	public static int GetDeaths (this PhotonPlayer player)
	{
		object teamId;
		if (player.customProperties.TryGetValue (PunPlayerDeaths.PlayerDeathsProp, out teamId)) {
			return (int)teamId;
		}

		return 0;
	}
}