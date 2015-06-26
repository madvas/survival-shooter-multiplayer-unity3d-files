using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;


static class PhotonPlayerExtensions
{
	public static readonly string PlayerDeathsProp = "d";
	public static readonly string PlayerMaterialProp = "m";

	public static void SetDeaths (this PhotonPlayer player, int newDeaths)
	{
		Hashtable deaths = new Hashtable ();  // using PUN's implementation of Hashtable
		deaths [PhotonPlayerExtensions.PlayerDeathsProp] = newDeaths;
		
		player.SetCustomProperties (deaths);  // this locally sets the deaths and will sync it in-game asap.
	}
	
	public static void AddDeaths (this PhotonPlayer player, int deathsToAddToCurrent)
	{
		int current = player.GetDeaths ();
		current = current + deathsToAddToCurrent;
		
		Hashtable deaths = new Hashtable ();  // using PUN's implementation of Hashtable
		deaths [PhotonPlayerExtensions.PlayerDeathsProp] = current;
		
		player.SetCustomProperties (deaths);  // this locally sets the deaths and will sync it in-game asap.
	}
	
	public static int GetDeaths (this PhotonPlayer player)
	{
		object teamId;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.PlayerDeathsProp, out teamId)) {
			return (int)teamId;
		}
		
		return 0;
	}

	public static void SetMaterialIndex (this PhotonPlayer player, int materialIndex)
	{
		Hashtable material = new Hashtable (); 
		material [PhotonPlayerExtensions.PlayerMaterialProp] = materialIndex;
		player.SetCustomProperties (material);
	}

	public static int GetMaterialIndex (this PhotonPlayer player)
	{
		object material;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.PlayerMaterialProp, out material)) {
			return (int)material;
		}
		
		return -1;
	}

	public static List<int> GetMaterials (this PhotonPlayer[] players)
	{
		List<int> materialList = new List<int> ();
		foreach (var player in players) {
			int matIdx = player.GetMaterialIndex ();
			if (matIdx > -1) {
				materialList.Add (matIdx);
			}
		}
		return materialList;
	}

}