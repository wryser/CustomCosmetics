using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;

namespace CustomCosmetics.Patches
{
    /// <summary>
    /// This is an example patch, made to demonstrate how to use Harmony. You should remove it if it is not used.
    /// </summary>
    [HarmonyPatch(typeof(CosmeticsController))]
    [HarmonyPatch("UpdateWornCosmetics", MethodType.Normal)]
    internal class UpdateWornPatch
    {
        private static void Postfix()
        {
            try
            {
                Plugin.instance.CheckItems();
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
