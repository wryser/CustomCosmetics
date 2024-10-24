using System;
using HarmonyLib;
using UnityEngine;

namespace CustomCosmetics.Patches
{
	// Token: 0x0200001A RID: 26
	[HarmonyPatch(typeof(VRRig))]
	[HarmonyPatch("ChangeMaterialLocal", 0)]
	internal class RigMaterialPatch
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00006D04 File Offset: 0x00004F04
		private static void Postfix(int materialIndex, VRRig __instance)
		{
			try
			{
				bool flag = __instance.isLocal && materialIndex != Plugin.instance.prevMatIndex;
				if (flag)
				{
					bool flag2 = materialIndex == 0;
					if (flag2)
					{
						Debug.Log("Set Material to default");
						Plugin.instance.prevMatIndex = materialIndex;
					}
					else
					{
						Debug.Log(string.Format("Material set to: {0}", materialIndex));
						Plugin.instance.prevMatIndex = materialIndex;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
