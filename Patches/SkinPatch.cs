using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCosmetics.Patches
{
    /// <summary>
    /// This is an example patch, made to demonstrate how to use Harmony. You should remove it if it is not used.
    /// </summary>
    [HarmonyPatch(typeof(GorillaSkin))]
    [HarmonyPatch("ShowSkin", MethodType.Normal)]
    internal class SkinPatch
    {
        private static void Postfix(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
        {
            try
            {
                if(useDefaultBodySkin && rig.isLocal)
                {
                    Plugin.instance.EnableMaterial();
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
