using HarmonyLib;

namespace CustomCosmetics.Patches
{
    [HarmonyPatch]
    public class RigCreatePatch
    {
        public static void Patch(NetPlayer player, VRRig vrrig)
        {
            Plugin.instance.RegisterPlayer(player, vrrig);
        }
    }
}