using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomCosmetics
{
    class HatLoadPage : WatchPage
    {
        public override string Title => "HatLoadPage";

        public override bool DisplayOnMainMenu => false;

        bool desetHat;
        HatDescriptor hat;

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
            selectionHandler.maxIndex = 0;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine($"<color=yellow>==</color> Hat <color=yellow>==</color>");
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
                str.AppendLine($"\n<color=red><size=0.40> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it to use the new features.</size></color>");
                str.AppendLines(2, "");
                if (Plugin.instance.hat.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Hat"));
                    desetHat = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Hat"));
                    desetHat = false;
                }
            }
            else
            {
                hat = Plugin.instance.hatDes;
                str.StartSize(0.65f);
                str.AppendLines(1);
                str.AppendLine("Name:");
                str.AppendLine(hat.Name);
                str.AppendLine("Author:");
                str.AppendLine(hat.Author);
                str.AppendLine("Description:");
                str.AppendLine(hat.Description);
                str.AppendLines(2);
                str.EndSize();
                str.AppendLines(2, "");
                if (Plugin.instance.hat.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Hat"));
                    desetHat = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Hat"));
                    desetHat = false;
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
                    if(selectionHandler.currentIndex == 0)
                    {
                        if (!desetHat)
                        {
                            Plugin.instance.LoadHat(Plugin.instance.currentCosmeticFile);
                        }
                        else
                        {
                            Plugin.instance.LoadHat("Disable");
                        }
                    }
                    OnGetScreenContent();
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(HatPage));
                    break;
            }
        }
    }
}
