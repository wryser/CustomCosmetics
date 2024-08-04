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
            str.AppendLine($"<color=yellow>==</color> Holdable <color=yellow>==</color>");
            if (Plugin.instance.usingTextMethod)
            {
                str.StartSize(0.65f);
                str.AppendLines(1);
                str.AppendLine("Name:");
                str.AppendLine(Plugin.instance.cosmeticName);
                str.AppendLine("Author:");
                str.AppendLine(Plugin.instance.cosmeticAuthor);
                str.AppendLine("Description:");
                str.AppendLine(Plugin.instance.cosmeticDescription);
                str.AppendLines(2);
                str.EndSize();
                str.AppendLine($"\n<color=red><size=0.40> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it.</size></color>");
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
                selectionHandler.currentIndex = 0;
            }
            else
            {
                holdable = Plugin.instance.holdableDes;
                str.StartSize(0.65f);
                str.AppendLines(1);
                str.AppendLine("Name:");
                str.AppendLine(holdable.Name);
                str.AppendLine("Author:");
                str.AppendLine(holdable.Author);
                str.AppendLine("Description:");
                str.AppendLine(holdable.Description);
                str.AppendLines(2);
                str.EndSize();
                str.AppendLines(2, "");
                if (Plugin.instance.Lholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Left Hand"));
                    desetLHand = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Left Hand"));
                    desetLHand = false;
                }

                if (Plugin.instance.Rholdable.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Unequip Right Hand"));
                    desetRHand = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Equip Right Hand"));
                    desetRHand = false;
                }
                selectionHandler.maxIndex = 1;
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
                    if(selectionHandler.currentIndex == 0)
                    {
                        if (!Plugin.instance.usingTextMethod)
                        {
                            if (!desetLHand)
                            {
                                Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile, true);
                            }
                            else
                            {
                                Plugin.instance.LoadHoldable("DisableL", true);
                            }
                        }
                        else
                        {
                            if (Plugin.instance.leftHand)
                            {
                                if (!desetLHand)
                                {
                                    Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile, true);
                                }
                                else
                                {
                                    Plugin.instance.LoadHoldable("DisableL", true);
                                }
                            }
                            else
                            {
                                if (!desetRHand)
                                {
                                    Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile, false);
                                }
                                else
                                {
                                    Plugin.instance.LoadHoldable("DisableR", false);
                                }
                            }
                        }
                    }
                    else if(selectionHandler.currentIndex == 1)
                    {
                        if (!desetRHand)
                        {
                            Plugin.instance.LoadHoldable(Plugin.instance.currentCosmeticFile, false);
                        }
                        else
                        {
                            Plugin.instance.LoadHoldable("DisableR", false);
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
