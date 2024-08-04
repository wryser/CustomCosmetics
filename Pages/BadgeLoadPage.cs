using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Mathematics;
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
            str.AppendLine($"<color=yellow>==</color> Badge <color=yellow>==</color>");
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
                str.StartSize(0.65f);
                str.AppendLines(1);
                str.AppendLine("Name:");
                str.AppendLine(badge.Name);
                str.AppendLine("Author:");
                str.AppendLine(badge.Author);
                str.AppendLine("Description:");
                str.AppendLine(badge.Description);
                str.AppendLines(2);
                str.EndSize();
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
