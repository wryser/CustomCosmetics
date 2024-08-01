using BananaOS;
using BananaOS.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomCosmetics
{
    class MaterialLoadPage : WatchPage
    {
        public override string Title => "MaterialLoadPage";

        public override bool DisplayOnMainMenu => false;

        bool desetMaterial;
        bool desetTagMaterial;
        MaterialDescriptor mat;

        public override void OnPostModSetup()
        {
            base.OnPostModSetup();
            selectionHandler.maxIndex = 1;
        }

        public override string OnGetScreenContent()
        {
            StringBuilder str = new StringBuilder();
            if (Plugin.instance.usingTextMethod)
            {
                str.AppendLine($"<size=0.60> Material Name: {Plugin.instance.cosmeticName}</size>");
                str.AppendLine($"<size=0.60> Material Author: {Plugin.instance.cosmeticAuthor}</size>");
                str.AppendLine($"<size=0.60> Material Description: {Plugin.instance.cosmeticDescription}</size>");
                if (Plugin.instance.materialCustomColours)
                {
                    str.AppendLine($"\n<size=0.50> Uses Custom Colours</size>");
                }
                str.AppendLine($"\n<color=red><size=0.70> This cosmetic is using the old descriptor system, this system is unsupported and has less features. If you made this cosmetic please update it to use the new features.</size></color>");
                str.AppendLines(2, "");
                if (Plugin.instance.material.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Unequip As Material"));
                    desetMaterial = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Equip As Material"));
                    desetMaterial = false;
                }

                if (Plugin.instance.taggedMaterial.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Unequip As Tagged Material"));
                    desetTagMaterial = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Equip As Tagged Material"));
                    desetTagMaterial = false;
                }
            }
            else
            {
                mat = Plugin.instance.matDes;
                str.AppendLine($"<size=0.60> Material Name: {mat.Name}</size>");
                str.AppendLine($"<size=0.60> Material Author: {mat.Author}</size>");
                str.AppendLine($"<size=0.60> Material Description: {mat.Description}</size>");
                if(mat.customColors)
                {
                    str.AppendLine($"\n<size=0.50> Uses Custom Colours</size>");
                }
                str.AppendLines(2, "");
                if (Plugin.instance.material.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Deset As Material"));
                    desetMaterial = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Set As Material"));
                    desetMaterial = false;
                }

                if (Plugin.instance.taggedMaterial.Value == Plugin.instance.currentCosmeticFile)
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Deset As Tagged Material"));
                    desetTagMaterial = true;
                }
                else
                {
                    str.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Set As Tagged Material"));
                    desetTagMaterial = false;
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
                        if (!desetMaterial)
                        {
                            Plugin.instance.LoadMaterial(Plugin.instance.currentCosmeticFile, 0);
                        }
                        else
                        {
                            Plugin.instance.LoadMaterial("Disable", 0);
                        }
                    }
                    else if(selectionHandler.currentIndex == 1)
                    {
                        if (!desetTagMaterial)
                        {
                            Plugin.instance.LoadMaterial(Plugin.instance.currentCosmeticFile, 2);
                        }
                        else
                        {
                            Plugin.instance.LoadMaterial("Disable", 2);
                        }
                    }
                    OnGetScreenContent();
                    break;
                case WatchButtonType.Back:
                    SwitchToPage(typeof(MaterialPage));
                    break;
            }
        }
    }
}
