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
            if (Plugin.instance.usingTextMethod)
            {
                str.AppendLine($"<size=0.60> Hat Name: {Plugin.instance.cosmeticName}</size>");
                str.AppendLine($"<size=0.60> Hat Author: {Plugin.instance.cosmeticAuthor}</size>");
                str.AppendLine($"<size=0.60> Hat Description: {Plugin.instance.cosmeticDescription}</size>");
                str.AppendLine($"\n<color=red><size=0.70> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it to use the new features.</size></color>");
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
                str.AppendLine($"<size=0.60> Hat Name: {hat.Name}</size>");
                str.AppendLine($"<size=0.60> Hat Author: {hat.Author}</size>");
                str.AppendLine($"<size=0.60> Hat Description: {hat.Description}</size>");
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
