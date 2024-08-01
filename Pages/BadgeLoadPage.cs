using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomCosmetics
{
    class BadgeLoadPage : WatchPage
    {
        public override string Title => "BadgeLoadPage";

        public override bool DisplayOnMainMenu => false;

        bool desetBadge;
        BadgeDescriptor badge;

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
                str.AppendLine($"<size=0.60> Badge Name: {Plugin.instance.cosmeticName}</size>");
                str.AppendLine($"<size=0.60> Badge Author: {Plugin.instance.cosmeticAuthor}</size>");
                str.AppendLine($"<size=0.60> Badge Description: {Plugin.instance.cosmeticDescription}</size>");
                str.AppendLine($"\n<color=red><size=0.70> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it to use the new features.</size></color>");
                str.AppendLines(2, "");
                if (Plugin.instance.badge.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Badge"));
                    desetBadge = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Badge"));
                    desetBadge = false;
                }
            }
            else
            {
                badge = Plugin.instance.badgeDes;
                str.AppendLine($"<size=0.60> Badge Name: {badge.Name}</size>");
                str.AppendLine($"<size=0.60> Badge Author: {badge.Author}</size>");
                str.AppendLine($"<size=0.60> Badge Description: {badge.Description}</size>");
                str.AppendLines(2, "");
                if (Plugin.instance.badge.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip Badge"));
                    desetBadge = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip Badge"));
                    desetBadge = false;
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
                        if (!desetBadge)
                        {
                            Plugin.instance.LoadBadge(Plugin.instance.currentCosmeticFile);
                        }
                        else
                        {
                            Plugin.instance.LoadBadge("Disable");
                        }
                    }
                    OnGetScreenContent();
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(BadgePage));
                    break;
            }
        }
    }
}
