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
            selectionHandler.maxIndex = 4;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            if (Plugin.instance.loadError) { return GetErrorContent(); }
            str.AppendLine("<color=yellow>==</color> Cosmetics <color=yellow>==</color>");
            str.AppendLines(1);
            if (Plugin.instance.assetsLoaded)
            {
                selectionHandler.maxIndex = 4;
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Holdables"));
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Hats"));
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, "Badges"));
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(3, "Materials"));
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(4, "Settings"));
            }
            else
            {
                str.AppendLine("Loading Cosmetics!");
            }
            return str.ToString();
        }

        string GetErrorContent()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<color=red>==Error When Loading==</color>");
            str.AppendLine("");
            str.AppendLine("There was an error when loading cosmetics.");
            str.AppendLine("Please make sure that you only have cosmetics installed from the discord server wryser's modding cave and not from the gorilla tag modding discord");
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
                    if(!Plugin.instance.assetsLoaded) { break; }
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
                        SwitchToPage(typeof(MaterialPage));
                    }
                    else if (index == 4)
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
