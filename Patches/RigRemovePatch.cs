using HarmonyLib;

namespace CustomCosmetics.Patches
{
    [HarmonyPatch]
    public class RigRemovePatch
    {
        public static void Patch(NetPlayer player, VRRig vrrig)
        {
            Plugin.instance.UnregisterPlayer(player, vrrig);
        }
    }
}