using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomCosmetics
{
    class HatPage : WatchPage
    {
        public override string Title => "Hats";

        public override bool DisplayOnMainMenu => false;
        List<string> hats = new List<string>();

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            string path = Plugin.instance.cosmeticPath + "/Hats";
            str.AppendLine("<color=yellow>==</color> Hats <color=yellow>==</color>");
            int i = 0;
            str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, "Disable Hat"));
            hats.Add("Disable");
            i++;
            foreach (string file in Directory.GetFiles(path))
            {
                string hatName = Path.GetFileNameWithoutExtension(file);
                str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(i, hatName));
                hats.Add(file);
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
                    Plugin.instance.LoadHat(hats[selectionHandler.currentIndex]);
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(CosmeticPage));
                    break;
            }
        }
    }
}
