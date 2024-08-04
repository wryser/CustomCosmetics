using HarmonyLib;

namespace CustomCosmetics.Patches
{
    [HarmonyPatch]
    public class RigJoinPatch
    {
        public static void Patch(NetPlayer player, VRRig vrrig)
        {
            Plugin.instance.CheckPlayer(player, vrrig);
        }
    }
}