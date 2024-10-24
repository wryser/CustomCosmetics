using System;
using CustomCosmetics.Networking;
using HarmonyLib;
using UnityEngine;

namespace CustomCosmetics.Patches
{
	// Token: 0x0200001C RID: 28
	[HarmonyPatch(typeof(GorillaSkin))]
	[HarmonyPatch("ShowSkin", 0)]
	internal class SkinPatch
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00006DF8 File Offset: 0x00004FF8
		private static void Postfix(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
		{
			try
			{
				if (useDefaultBodySkin && rig.isLocal)
				{
					Plugin.instance.EnableMaterial();
				}
				else if (useDefaultBodySkin)
				{
					NetworkingBehaviours.EnableNetworkMaterial(rig);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
