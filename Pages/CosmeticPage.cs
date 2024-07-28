using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCosmetics
{
    class CosmeticPage : WatchPage
    {
        public override string Title => "Cosmetics";

        public override bool DisplayOnMainMenu => true;

        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 3;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<color=yellow>==</color> Cosmetics <color=yellow>==</color>");
            str.AppendLines(1);
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Holdables"));
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Hats"));
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, "Badges"));
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(3, "Settings"));
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
                    if (index == 0)
                    {
                        SwitchToPage(typeof(HoldablePage));
                    }
                    else if(index == 1)
                    {
                        SwitchToPage(typeof(HatPage));
                    }
                    else if (index == 2)
                    {
                        SwitchToPage(typeof(BadgePage));
                    }
                    else if (index == 3)
                    {
                        SwitchToPage(typeof(SettingsPage));
                    }
                    break;
                case WatchButtonType.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
    }
}
