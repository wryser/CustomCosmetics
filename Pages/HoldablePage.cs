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

        string path = Plugin.instance.cosmeticPath + "/Holdables";
        List<string> holdables = new List<string>();
        int pageNum = 1;
        int currentSelected = 0;
        int maxCurrentPage;
        int minCurrentPage;
        double maxPages;

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
            foreach (string file in Directory.GetFiles(path))
            {
                holdables.Add(file);
            }
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            if (holdables.Count <= 0)
            {
                str.AppendLine($"No {Title} installed");
                return str.ToString();
            }
            double mathstuff = Convert.ToDouble(holdables.Count) / 9.0;
            maxPages = Math.Ceiling(mathstuff);
            str.AppendLine("<color=yellow>==</color> Holdables <color=yellow>==</color>");
            str.AppendLines(1);
            if(pageNum == maxPages)
            {
                int t = 9 * pageNum - holdables.Count;
                int ii = 0;
                minCurrentPage = 9 * pageNum - 9;
                for (int i = 9 * pageNum - 9; i < 9 * pageNum - t; i++)
                {
                    string holdableName = Path.GetFileNameWithoutExtension(holdables[i]);
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(ii, holdableName));
                    ii++;
                    minCurrentPage++;
                }
                maxCurrentPage = minCurrentPage - 1;
                minCurrentPage = 9 * pageNum - 9;
                selectionHandler.maxIndex = ii - 1;
            }
            else
            {
                int ii = 0;
                int t = 9 * pageNum - 9;
                for (int i = 9 * pageNum - 9; i < 9 * pageNum; i++)
                {
                    string holdableName = Path.GetFileNameWithoutExtension(holdables[i]);
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(ii, holdableName));
                    ii++;
                    minCurrentPage++;
                }
                maxCurrentPage = minCurrentPage - 1;
                minCurrentPage = 9 * pageNum - 9;
                selectionHandler.maxIndex = 8;
            }
            str.AppendLine($"{pageNum}/{maxPages}");
            return str.ToString();
        }

        public override void OnButtonPressed(WatchButtonType buttonType)
        {
            switch (buttonType)
            {
                case WatchButtonType.Up:
                    selectionHandler.MoveSelectionUp();
                    currentSelected--;
                    if (currentSelected < 0)
                    {
                        currentSelected = maxCurrentPage;
                    }
                    break;
                case WatchButtonType.Down:
                    selectionHandler.MoveSelectionDown();
                    currentSelected++;
                    if (currentSelected > maxCurrentPage)
                    {
                        currentSelected = minCurrentPage;
                    }
                    break;
                case WatchButtonType.Left:
                    pageNum--;
                    if (pageNum < 1)
                    {
                        pageNum = (int)maxPages;
                    }
                    OnGetScreenContent();
                    currentSelected = minCurrentPage;
                    selectionHandler.currentIndex = 0;
                    break;
                case WatchButtonType.Right:
                    pageNum++;
                    if (pageNum > maxPages)
                    {
                        pageNum = 1;
                    }
                    OnGetScreenContent();
                    currentSelected = minCurrentPage;
                    selectionHandler.currentIndex = 0;
                    break;
                case WatchButtonType.Enter:
                    Plugin.instance.GetInfo(Path.GetFileName(holdables[currentSelected]), "Holdable");
                    SwitchToPage(typeof(HoldableLoadPage));
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(CosmeticPage));
                    break;
            }
        }
    }
}
