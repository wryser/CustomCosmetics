using System;
using HarmonyLib;

namespace CustomCosmetics.Patches
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch]
	public class RigLeavePatch
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00006CEA File Offset: 0x00004EEA
		public static void Patch(NetPlayer player, VRRig vrrig)
		{
			Plugin.instance.RemovePlayer(player, vrrig);
		}
	}
}
