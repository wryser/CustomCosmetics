using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomCosmetics
{
    class SettingsPage : WatchPage
    {
        public override string Title => "Settings";

        public override bool DisplayOnMainMenu => false;

        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 2;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<color=yellow>==</color> Settings <color=yellow>==</color>");
            str.AppendLines(1);
            if(Plugin.instance.GetMultipleHats() == false)
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "<color=red> Multiple Hats </color>"));
                str.AppendLine("<size=0.40> Allow wearing multiple hats at once?</size>");
            }
            else
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "<color=green> Multiple Hats </color>"));
                str.AppendLine("<size=0.40> Allow wearing multiple hats at once?</size>");
            }
            if (Plugin.instance.GetRemoveCosmetics() == false)
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "<color=red> Unequip Normal Cosmetics </color>"));
                str.AppendLine("<size=0.40> Unequip normal cosmetics when equipping custom ones?</size>");
            }
            else
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "<color=green> Unequip Normal Cosmetics </color>"));
                str.AppendLine("<size=0.40> Unequip normal cosmetics when equipping custom ones?</size>");
            }
            str.AppendLine("");
            str.AppendLine("<color=yellow>Networking</color>");
            if(Plugin.instance.GetMaxFileSize() == 0)
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, $"<color=white> Max File Size: No Limit </color>"));
                str.AppendLine("<size=0.40> File size limit to load when loading other peoples custom cosmetics.</size>");
            }
            else
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, $"<color=white> Max File Size (kb): {Plugin.instance.GetMaxFileSize()} </color>"));
                str.AppendLine("<size=0.40> File size limit to load when loading other peoples custom cosmetics.</size>");
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
                case WatchButtonType.Right:
                    int i = selectionHandler.currentIndex;
                    if(i == 2)
                    {
                        Plugin.instance.ChangeMaxFileSize(false);
                        OnGetScreenContent();
                    }
                    break;
                case WatchButtonType.Left:
                    int t = selectionHandler.currentIndex;
                    if (t == 2)
                    {
                        Plugin.instance.ChangeMaxFileSize(true);
                        OnGetScreenContent();
                    }
                    break;
                case WatchButtonType.Enter:
                    int index = selectionHandler.currentIndex;
                    if(index == 0)
                    {
                        Plugin.instance.ToggleMultipleHats();
                    }
                    if (index == 1)
                    {
                        Plugin.instance.ToggleRemoveCosmetics();
                    }
                    OnGetScreenContent();
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(CosmeticPage));
                    break;
            }
        }
    }
}
