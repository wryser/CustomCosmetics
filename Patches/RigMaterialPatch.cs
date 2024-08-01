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
    [HarmonyPatch(typeof(VRRig))]
    [HarmonyPatch("ChangeMaterialLocal", MethodType.Normal)]
    internal class RigMaterialPatch
    {
        private static void Postfix(int materialIndex, VRRig __instance)
        {
            try
            {
                if (__instance.isLocal && materialIndex != Plugin.instance.prevMatIndex)
                {
                    if (materialIndex == 0)
                    {
                        Debug.Log("Set Material to default");
                        Plugin.instance.prevMatIndex = materialIndex;
                    }
                    else
                    {
                        Debug.Log($"Material set to: {materialIndex}");
                        Plugin.instance.prevMatIndex = materialIndex;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
