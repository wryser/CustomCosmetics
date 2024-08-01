using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomCosmetics
{
    class HoldableLoadPage : WatchPage
    {
        public override string Title => "HoldableLoadPage";

        public override bool DisplayOnMainMenu => false;

        bool desetLHand;
        bool desetRHand;
        HoldableDescriptor holdable;

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
            selectionHandler.maxIndex = 1;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            if (Plugin.instance.usingTextMethod)
            {
                str.AppendLine($"<size=0.60> Holdable Name: {Plugin.instance.cosmeticName}</size>");
                str.AppendLine($"<size=0.60> Holdable Author: {Plugin.instance.cosmeticAuthor}</size>");
                str.AppendLine($"<size=0.60> Holdable Description: {Plugin.instance.cosmeticDescription}</size>");
                str.AppendLine($"\n<color=red><size=0.70> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it.</size></color>");
                str.AppendLines(2, "");
                if (Plugin.instance.leftHand && Plugin.instance.Lholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Deset Left Hand"));
                    desetLHand = true;
                }
                else if (Plugin.instance.leftHand)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Set Left Hand"));
                    desetLHand = false;
                }

                if (!Plugin.instance.leftHand && Plugin.instance.Rholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Deset Right Hand"));
                    desetRHand = true;
                }
                else if (!Plugin.instance.leftHand)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Set Right Hand"));
                    desetRHand = false;
                }
            }
            else
            {
                holdable = Plugin.instance.holdableDes;
                str.AppendLine($"<size=0.60> Holdable Name: {holdable.Name}</size>");
                str.AppendLine($"<size=0.60> Holdable Author: {holdable.Author}</size>");
                str.AppendLine($"<size=0.60> Holdable Description: {holdable.Description}</size>");
                str.AppendLines(2, "");
                if (Plugin.instance.leftHand && Plugin.instance.Lholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Left Hand"));
                    desetLHand = true;
                }
                else if (Plugin.instance.leftHand)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Left Hand"));
                    desetLHand = false;
                }

                if (!Plugin.instance.leftHand && Plugin.instance.Rholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Right Hand"));
                    desetRHand = true;
                }
                else if (!Plugin.instance.leftHand)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Right Hand"));
                    desetRHand = false;
                }
            }
            return str.ToString();
        }
        public override void OnButtonPressed(WatchButtonType buttonType)
        {
            switch (buttonType)
            {
                case WatchButtonType.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case WatchButtonType.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case WatchButtonType.Enter:
                    if(Plugin.instance.leftHand)
                    {
                        if (!desetLHand)
                        {
                            Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile);
                        }
                        else
                        {
                            Plugin.instance.LoadHoldable("DisableL");
                        }
                    }
                    else
                    {
                        if (!desetRHand)
                        {
                            Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile);
                        }
                        else
                        {
                            Plugin.instance.LoadHoldable("DisableR");
                        }
                    }
                    OnGetScreenContent();
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(HoldablePage));
                    break;
            }
        }
    }
}
