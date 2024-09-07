using CustomCosmetics;
using Photon.Pun;
using UnityEngine;

class Net : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer.IsLocal) return;

        Debug.Log("Updating Players Cosmetics");
        Plugin.instance.RemoveCosmetics(changedProps, GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), targetPlayer);
        Plugin.instance.SetCosmetics(GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), changedProps, targetPlayer);
    }
}