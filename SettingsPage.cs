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
            selectionHandler.maxIndex = 0;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<color=yellow>==</color> Settings <color=yellow>==</color>");
            str.AppendLines(1);
            if(Plugin.instance.GetMultipleHats() == false)
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "<color=red> Multiple Hats </color>"));
            }
            else
            {
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "<color=green> Multiple Hats </color>"));
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
                    int index = selectionHandler.currentIndex;
                    if(index == 0)
                    {
                        Plugin.instance.ToggleMultipleHats();
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
