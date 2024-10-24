using System;
using System.Collections.Generic;
using CustomCosmetics.Extensions;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCosmetics.Networking
{
	// Token: 0x0200001E RID: 30
	internal class NetworkingBehaviours
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00006EA8 File Offset: 0x000050A8
		public static void EnableNetworkMaterial(VRRig rig)
		{
			Player player;
			bool flag = NetworkingBehaviours.cosmeticsplayers.TryGetValue(rig, out player);
			if (flag)
			{
				object obj;
				bool flag2 = player.CustomProperties.TryGetValue("CustomMaterial", out obj);
				if (flag2)
				{
					object obj2;
					player.CustomProperties.TryGetValue("Colour", out obj2);
					Color color = ColorExtensions.parseColor(obj2.ToString());
					NetworkingBehaviours.LoadNetworkMaterial(obj.ToString(), 0, rig, player, color);
				}
				object obj3;
				bool flag3 = player.CustomProperties.TryGetValue("CustomTagMaterial", out obj3);
				if (flag3)
				{
					object obj4;
					player.CustomProperties.TryGetValue("Colour", out obj4);
					Color color2 = ColorExtensions.parseColor(obj4.ToString());
					NetworkingBehaviours.LoadNetworkMaterial(obj3.ToString(), 2, rig, player, color2);
				}
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006F64 File Offset: 0x00005164
		public static void LoadNetworkHat(string file, VRRig rig, Player player)
		{
			bool flag = file != "";
			if (flag)
			{
				GameObject gameObject;
				NetworkingBehaviours.nameAssetCache.TryGetValue(file, out gameObject);
				GameObject gameObject2 = Plugin.Instantiate<GameObject>(gameObject);
				bool flag2 = gameObject2 != null;
				if (flag2)
				{
					GameObject gameObject3 = gameObject2;
					foreach (Collider collider in gameObject3.GetComponentsInChildren<Collider>())
					{
						Plugin.Destroy(collider);
					}
					NetworkingBehaviours.networkHats.Add(player, gameObject3);
					gameObject3.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/head/"), false);
				}
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00007008 File Offset: 0x00005208
		public static void LoadNetworkHoldable(string file, VRRig rig, Player player, bool lHand)
		{
			try
			{
				bool flag = file != "";
				if (flag)
				{
					GameObject gameObject;
					NetworkingBehaviours.nameAssetCache.TryGetValue(file, out gameObject);
					GameObject gameObject2 = Plugin.Instantiate<GameObject>(gameObject);
					bool flag2 = gameObject2 != null;
					if (flag2)
					{
						GameObject gameObject3 = gameObject2;
						foreach (Collider collider in gameObject3.GetComponentsInChildren<Collider>())
						{
							Plugin.Destroy(collider);
						}
						bool flag3 = gameObject3.TryGetComponent<Text>(out Text text);
						if (flag3)
						{
							bool flag4 = !lHand;
							if (flag4)
							{
								gameObject3.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
								NetworkingBehaviours.networkRHoldables.Add(player, gameObject3);
							}
							else if (lHand)
							{
								gameObject3.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
								NetworkingBehaviours.networkLHoldables.Add(player, gameObject3);
							}
						}
						else
						{
							bool flag5 = !lHand;
							if (flag5)
							{
								GameObject gameObject4 = Plugin.Instantiate<GameObject>(gameObject3.GetComponent<HoldableDescriptor>().rightHandObject);
								gameObject4.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
								NetworkingBehaviours.networkRHoldables.Add(player, gameObject4);
							}
							else if (lHand)
							{
								GameObject gameObject5 = Plugin.Instantiate<GameObject>(gameObject3.GetComponent<HoldableDescriptor>().leftHandObject);
								gameObject5.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
								NetworkingBehaviours.networkLHoldables.Add(player, gameObject5);
							}
							Plugin.Destroy(gameObject3);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000071D8 File Offset: 0x000053D8
		public static void LoadNetworkBadge(string file, VRRig rig, Player player)
		{
			bool flag = file != "";
			if (flag)
			{
				GameObject gameObject;
				NetworkingBehaviours.nameAssetCache.TryGetValue(file, out gameObject);
				GameObject gameObject2 = Plugin.Instantiate<GameObject>(gameObject);
				bool flag2 = gameObject2 != null;
				if (flag2)
				{
					GameObject gameObject3 = gameObject2;
					foreach (Collider collider in gameObject3.GetComponentsInChildren<Collider>())
					{
						Plugin.Destroy(collider);
					}
					gameObject3.transform.SetParent(rig.transform.Find("RigAnchor/rig/body/"), false);
					NetworkingBehaviours.networkBadges.Add(player, gameObject3);
				}
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000727C File Offset: 0x0000547C
		public static void LoadNetworkMaterial(string file, int materialIndex, VRRig rig, Player player, Color colour)
		{
			bool flag = file != "";
			if (flag)
			{
				GameObject gameObject;
				NetworkingBehaviours.nameAssetCache.TryGetValue(file, out gameObject);
				GameObject gameObject2 = Plugin.Instantiate<GameObject>(gameObject);
				bool flag2 = gameObject2 != null;
				if (flag2)
				{
					GameObject gameObject3 = gameObject2;
					try
					{
						bool flag3 = materialIndex == 0;
						if (flag3)
						{
							MaterialDescriptor component = gameObject3.GetComponent<MaterialDescriptor>();
							bool customColors = component.customColors;
							if (customColors)
							{
								gameObject3.GetComponent<MeshRenderer>().material.color = rig.playerColor;
							}
							rig.materialsToChangeTo[0] = gameObject3.GetComponent<MeshRenderer>().material;
							Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
							sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
							sharedMaterials[1] = rig.defaultSkin.chestMaterial;
							rig.mainSkin.sharedMaterials = sharedMaterials;
						}
						else
						{
							bool flag4 = materialIndex == 2;
							if (flag4)
							{
								MaterialDescriptor component2 = gameObject3.GetComponent<MaterialDescriptor>();
								rig.materialsToChangeTo[materialIndex] = gameObject3.GetComponent<MeshRenderer>().material;
								Material[] sharedMaterials2 = rig.mainSkin.sharedMaterials;
								sharedMaterials2[0] = rig.materialsToChangeTo[rig.setMatIndex];
								sharedMaterials2[1] = rig.defaultSkin.chestMaterial;
								rig.mainSkin.sharedMaterials = sharedMaterials2;
							}
							else
							{
								rig.materialsToChangeTo[materialIndex] = NetworkingBehaviours.currentTaggedMaterial.mat;
							}
						}
						Plugin.Destroy(gameObject3);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
			else
			{
				bool flag5 = materialIndex == 0;
				if (flag5)
				{
					rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
					Material[] sharedMaterials3 = rig.mainSkin.sharedMaterials;
					sharedMaterials3[0] = rig.materialsToChangeTo[rig.setMatIndex];
					sharedMaterials3[1] = rig.defaultSkin.chestMaterial;
					rig.mainSkin.sharedMaterials = sharedMaterials3;
				}
				else
				{
					bool flag6 = materialIndex == 2;
					if (flag6)
					{
						rig.materialsToChangeTo[2] = NetworkingBehaviours.defaultTaggedMaterial;
						Material[] sharedMaterials4 = rig.mainSkin.sharedMaterials;
						sharedMaterials4[0] = rig.materialsToChangeTo[rig.setMatIndex];
						sharedMaterials4[1] = rig.defaultSkin.chestMaterial;
						rig.mainSkin.sharedMaterials = sharedMaterials4;
					}
				}
			}
		}

		// Token: 0x0400006D RID: 109
		public static Dictionary<VRRig, Player> cosmeticsplayers = new Dictionary<VRRig, Player>();

		// Token: 0x0400006E RID: 110
		public static Dictionary<string, GameObject> assetCache = new Dictionary<string, GameObject>();

		// Token: 0x0400006F RID: 111
		public static Dictionary<string, GameObject> nameAssetCache = new Dictionary<string, GameObject>();

		// Token: 0x04000070 RID: 112
		public static GameObject currentRHoldable;

		// Token: 0x04000071 RID: 113
		public static GameObject currentLHoldable;

		// Token: 0x04000072 RID: 114
		public static GameObject currentHat;

		// Token: 0x04000073 RID: 115
		public static GameObject currentBadge;

		// Token: 0x04000074 RID: 116
		public static NetworkingBehaviours.customMaterial currentMaterial = default(NetworkingBehaviours.customMaterial);

		// Token: 0x04000075 RID: 117
		public static NetworkingBehaviours.customMaterial currentTaggedMaterial = default(NetworkingBehaviours.customMaterial);

		// Token: 0x04000076 RID: 118
		public static Material defaultTaggedMaterial;

		// Token: 0x04000077 RID: 119
		public static Dictionary<Player, GameObject> networkHats = new Dictionary<Player, GameObject>();

		// Token: 0x04000078 RID: 120
		public static Dictionary<Player, GameObject> networkRHoldables = new Dictionary<Player, GameObject>();

		// Token: 0x04000079 RID: 121
		public static Dictionary<Player, GameObject> networkLHoldables = new Dictionary<Player, GameObject>();

		// Token: 0x0400007A RID: 122
		public static Dictionary<Player, GameObject> networkBadges = new Dictionary<Player, GameObject>();

		// Token: 0x02000026 RID: 38
		public struct customMaterial
		{
			// Token: 0x040000AB RID: 171
			public Material mat;

			// Token: 0x040000AC RID: 172
			public bool customColours;
		}
	}
}
