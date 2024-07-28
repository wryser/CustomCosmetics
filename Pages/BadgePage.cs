using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomCosmetics
{
    class BadgePage : WatchPage
    {
        public override string Title => "Badges";

        public override bool DisplayOnMainMenu => false;
        List<string> badges = new List<string>();

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            string path = Plugin.instance.cosmeticPath + "/Badges";
            str.AppendLine("<color=yellow>==</color> Badges <color=yellow>==</color>");
            int i = 0;
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, "Disable Badge"));
            badges.Add("Disable");
            i++;
            foreach (string file in Directory.GetFiles(path))
            {
                string badgeName = Path.GetFileNameWithoutExtension(file);
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, badgeName));
                badges.Add(file);
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
                    Plugin.instance.LoadBadge(badges[selectionHandler.currentIndex]);
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(CosmeticPage));
                    break;
            }
        }
    }
}
