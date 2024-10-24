using System;
using HarmonyLib;
using UnityEngine;

namespace CustomCosmetics.Patches
{
	// Token: 0x02000017 RID: 23
	[HarmonyPatch(typeof(VRRig), "InitializeNoobMaterialLocal")]
	internal class InitializeNoobMaterial
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00006C78 File Offset: 0x00004E78
		private static void Postfix(VRRig __instance, float red, float green, float blue)
		{
			try
			{
				bool isLocal = __instance.isLocal;
				if (isLocal)
				{
					Color color = new Color(red, green, blue);
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
