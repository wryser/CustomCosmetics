using System;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;

namespace CustomCosmetics.Patches
{
	// Token: 0x0200001D RID: 29
	[HarmonyPatch(typeof(CosmeticsController))]
	[HarmonyPatch("UpdateWornCosmetics", 0)]
	internal class UpdateWornPatch
	{
		// Token: 0x0600006C RID: 108 RVA: 0x00006E60 File Offset: 0x00005060
		private static void Postfix()
		{
			try
			{
				Plugin.instance.CheckItems();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
