using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (Plugin.instance.loadError)
            {
                selectionHandler.maxIndex = 0;
                str.Append(Plugin.instance.errorText.ToString());
                str.AppendLine("");
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Delete"));
                selectionHandler.currentIndex = 0;
                return str.ToString();
            }
            if (!Plugin.instance.correctVersion)
            {
                selectionHandler.maxIndex = 0;
                str.Append(Plugin.instance.errorText.ToString());
                selectionHandler.currentIndex = 0;
                return str.ToString();
            }
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

        public override void OnButtonPressed(WatchButtonType buttonType)
        {
            switch (buttonType)
            {
                case WatchButtonType.Up:
                    if(!Plugin.instance.loadError && Plugin.instance.correctVersion)
                    {
                        selectionHandler.MoveSelectionUp();
                    }
                    break;
                case WatchButtonType.Down:
                    if (!Plugin.instance.loadError && Plugin.instance.correctVersion)
                    {
                        selectionHandler.MoveSelectionDown();
                    }
                    break;
                case WatchButtonType.Enter:
                    int index = selectionHandler.currentIndex;
                    if (Plugin.instance.loadError)
                    {
                        File.Delete(Plugin.instance.brokenCosmetic);
                        Plugin.instance.LoadAssets();
                        OnGetScreenContent();
                    }
                    if (!Plugin.instance.assetsLoaded)
                    {
                        break;
                    }
                    if (!Plugin.instance.correctVersion)
                    {
                        break;
                    }
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
