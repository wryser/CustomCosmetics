using System;
using CustomCosmetics;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000007 RID: 7
internal class Net : MonoBehaviourPunCallbacks
{
	// Token: 0x06000006 RID: 6 RVA: 0x00002154 File Offset: 0x00000354
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
		bool isLocal = targetPlayer.IsLocal;
		if (!isLocal)
		{
			Debug.Log("Updating Players Cosmetics");
			Plugin.instance.RemoveCosmetics(changedProps, GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), targetPlayer);
			Plugin.instance.SetCosmetics(GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), changedProps, targetPlayer);
		}
	}
}
