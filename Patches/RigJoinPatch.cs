using System;
using HarmonyLib;

namespace CustomCosmetics.Patches
{
	// Token: 0x02000018 RID: 24
	[HarmonyPatch]
	public class RigJoinPatch
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00006CD1 File Offset: 0x00004ED1
		public static void Patch(NetPlayer player, VRRig vrrig)
		{
			Plugin.instance.CheckPlayer(player, vrrig);
		}
	}
}
