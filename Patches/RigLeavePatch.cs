using HarmonyLib;

namespace CustomCosmetics.Patches
{
    [HarmonyPatch]
    public class RigLeavePatch
    {
        public static void Patch(NetPlayer player, VRRig vrrig)
        {
            Plugin.instance.RemovePlayer(player, vrrig);
        }
    }
}