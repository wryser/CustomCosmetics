using System;
using CustomCosmetics.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCosmetics.Extensions
{
	// Token: 0x02000021 RID: 33
	internal class InfoLoader
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00007650 File Offset: 0x00005850
		public static void GetInfo(string file, string mode)
		{
			GameObject gameObject;
			NetworkingBehaviours.assetCache.TryGetValue(file, out gameObject);
			bool flag = gameObject.TryGetComponent<Text>(out Text text);
			if (flag)
			{
				Plugin.instance.usingTextMethod = true;
				string[] array = text.text.Split("$", StringSplitOptions.None);
				if (!(mode == "Material"))
				{
					if (!(mode == "Holdable"))
					{
						if (!(mode == "Badge"))
						{
							if (mode == "Hat")
							{
								Plugin.instance.currentCosmeticFile = file;
								Plugin.instance.cosmeticName = array[0];
								Plugin.instance.cosmeticAuthor = array[1];
								Plugin.instance.cosmeticDescription = array[2];
							}
						}
						else
						{
							Plugin.instance.currentCosmeticFile = file;
							Plugin.instance.cosmeticName = array[0];
							Plugin.instance.cosmeticAuthor = array[1];
							Plugin.instance.cosmeticDescription = array[2];
						}
					}
					else
					{
						Plugin.instance.currentCosmeticFile = file;
						Plugin.instance.cosmeticName = array[0];
						Plugin.instance.cosmeticAuthor = array[1];
						Plugin.instance.cosmeticDescription = array[2];
						Plugin.instance.leftHand = array[3].ToUpper() == "TRUE";
					}
				}
				else
				{
					Plugin.instance.currentCosmeticFile = file;
					Plugin.instance.cosmeticName = array[0];
					Plugin.instance.cosmeticAuthor = array[1];
					Plugin.instance.cosmeticDescription = array[2];
					Plugin.instance.materialCustomColours = array[3].ToUpper() == "TRUE";
				}
			}
			else
			{
				Plugin.instance.usingTextMethod = false;
				if (!(mode == "Material"))
				{
					if (!(mode == "Holdable"))
					{
						if (!(mode == "Hat"))
						{
							if (mode == "Badge")
							{
								Plugin.instance.currentCosmeticFile = file;
								Plugin.instance.badgeDes = gameObject.GetComponent<BadgeDescriptor>();
							}
						}
						else
						{
							Plugin.instance.currentCosmeticFile = file;
							Plugin.instance.hatDes = gameObject.GetComponent<HatDescriptor>();
						}
					}
					else
					{
						Plugin.instance.currentCosmeticFile = file;
						Plugin.instance.holdableDes = gameObject.GetComponent<HoldableDescriptor>();
					}
				}
				else
				{
					Plugin.instance.currentCosmeticFile = file;
					Plugin.instance.matDes = gameObject.GetComponent<MaterialDescriptor>();
				}
			}
		}
	}
}
