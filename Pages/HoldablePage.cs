using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace CustomCosmetics
{
    class HoldablePage : WatchPage
    {
        public override string Title => "Holdables";

        public override bool DisplayOnMainMenu => false;
        List<string> holdables = new List<string>();

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();

        }
        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            string path = Plugin.instance.cosmeticPath + "/Holdables";
            str.AppendLine("<color=yellow>==</color> Holdables <color=yellow>==</color>");
            str.AppendLines(1);
            int i = 0;
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, "Disable R Holdable"));
            holdables.Add("DisableR");
            i++;
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, "Disable L Holdable"));
            holdables.Add("DisableL");
            i++;
            foreach (string file in Directory.GetFiles(path))
            {
                string holdableName = Path.GetFileNameWithoutExtension(file);
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, holdableName));
                holdables.Add(file);
                i++;
            }
            selectionHandler.maxIndex = i - 1;
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
                    Plugin.instance.LoadHoldable(holdables[selectionHandler.currentIndex]);
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(CosmeticPage));
                    break;
            }
        }
    }
}
