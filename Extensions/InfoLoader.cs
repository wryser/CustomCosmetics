using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static CustomCosmetics.Networking.NetworkingBehaviours;

namespace CustomCosmetics.Extensions
{
    class InfoLoader
    {
        public static void GetInfo(string file, string mode)
        {
            GameObject cosmetic;
            string[] info;
            assetCache.TryGetValue(file, out cosmetic);

            if (cosmetic.TryGetComponent(out Text values))
            {
                Plugin.instance.usingTextMethod = true;
                info = values.text.Split("$");
                switch (mode)
                {
                    case "Material":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.cosmeticName = info[0];
                        Plugin.instance.cosmeticAuthor = info[1];
                        Plugin.instance.cosmeticDescription = info[2];
                        Plugin.instance.materialCustomColours = info[3].ToUpper() == "TRUE";
                        break;
                    case "Holdable":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.cosmeticName = info[0];
                        Plugin.instance.cosmeticAuthor = info[1];
                        Plugin.instance.cosmeticDescription = info[2];
                        Plugin.instance.leftHand = info[3].ToUpper() == "TRUE";
                        break;
                    case "Badge":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.cosmeticName = info[0];
                        Plugin.instance.cosmeticAuthor = info[1];
                        Plugin.instance.cosmeticDescription = info[2];
                        break;
                    case "Hat":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.cosmeticName = info[0];
                        Plugin.instance.cosmeticAuthor = info[1];
                        Plugin.instance.cosmeticDescription = info[2];
                        break;
                }
            }
            else
            {
                Plugin.instance.usingTextMethod = false;
                switch (mode)
                {
                    case "Material":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.matDes = cosmetic.GetComponent<MaterialDescriptor>();
                        break;
                    case "Holdable":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.holdableDes = cosmetic.GetComponent<HoldableDescriptor>();
                        break;
                    case "Hat":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.hatDes = cosmetic.GetComponent<HatDescriptor>();
                        break;
                    case "Badge":
                        Plugin.instance.currentCosmeticFile = file;
                        Plugin.instance.badgeDes = cosmetic.GetComponent<BadgeDescriptor>();
                        break;
                }
            }
        }
    }
}
