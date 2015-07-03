using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

static class PhotonPlayerExtensions
{
	public static readonly string deathsProp = "d";
	public static readonly string materialProp = "m";
	public static readonly string increasedDamageProp = "g";
	public static readonly string invisibilityProp = "i";

	private static void SetProperty<T> (this PhotonPlayer player, string key, T value)
	{
		Hashtable prop = new Hashtable ();
		prop [key] = value;
		player.SetCustomProperties (prop);
	}

	public static void SetDeaths (this PhotonPlayer player, int newDeaths)
	{
		player.SetProperty (PhotonPlayerExtensions.deathsProp, newDeaths);
	}
	
	public static void AddDeaths (this PhotonPlayer player, int deathsToAddToCurrent)
	{
		int current = player.GetDeaths ();
		current = current + deathsToAddToCurrent;
		player.SetProperty (PhotonPlayerExtensions.deathsProp, current);
	}
	
	public static int GetDeaths (this PhotonPlayer player)
	{
		object teamId;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.deathsProp, out teamId)) {
			return (int)teamId;
		}
		
		return 0;
	}

	public static void SetMaterialIndex (this PhotonPlayer player, int materialIndex)
	{
		player.SetProperty (PhotonPlayerExtensions.materialProp, materialIndex);
	}

	public static int GetMaterialIndex (this PhotonPlayer player)
	{
		object material;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.materialProp, out material)) {
			return (int)material;
		}
		
		return -1;
	}

	public static void SetIncreasedDamage (this PhotonPlayer player, bool enabled)
	{
		player.SetProperty (PhotonPlayerExtensions.increasedDamageProp, enabled);
	}

	public static bool HasIncreasedDamage (this PhotonPlayer player)
	{
		object hasIncreased;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.increasedDamageProp, out hasIncreased)) {
			return (bool)hasIncreased;
		}
		return false;
	}

	public static void SetInvisibility (this PhotonPlayer player, bool enabled)
	{
		player.SetProperty (PhotonPlayerExtensions.invisibilityProp, enabled);
	}
	
	public static bool isInvisible (this PhotonPlayer player)
	{
		object isInvisible;
		if (player.customProperties.TryGetValue (PhotonPlayerExtensions.invisibilityProp, out isInvisible)) {
			return (bool)isInvisible;
		}
		return false;
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

	public static PhotonPlayer[] GetOtherPlayers (this PhotonPlayer[] players)
	{
		return players.Where (p => p.ID != PhotonNetwork.player.ID).ToArray ();
	}
}