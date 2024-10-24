using System;
using HarmonyLib;
using UnityEngine;

namespace CustomCosmetics.Patches
{
	// Token: 0x0200001B RID: 27
	[HarmonyPatch(typeof(VRRig))]
	[HarmonyPatch("SetColor", 0)]
	internal class SetColor
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00006DA4 File Offset: 0x00004FA4
		private static void Postfix(VRRig __instance, Color color)
		{
			try
			{
				bool isLocal = __instance.isLocal;
				if (isLocal)
				{
					Plugin.instance.UpdateColour(color);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
