using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BananaOS;
using BepInEx;
using BepInEx.Configuration;
using CustomCosmetics.Extensions;
using CustomCosmetics.Networking;
using CustomCosmetics.Patches;
using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomCosmetics
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			SceneManager.sceneLoaded += GameInitialized;
			instance = this;
		}

		private IEnumerator checkVersion()
		{
			UnityWebRequest www = UnityWebRequest.Get(PluginInfo.DevVersionCheck);
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(www.error);
			}
			else
			{
				string[] info = www.downloadHandler.text.Split("$", StringSplitOptions.None);
				Debug.Log("Version is: " + info[0] + " Discord link is: " + info[1]);
				Debug.Log("Got Version!");
				if (info[0] != "1.2.5.0")
				{
					BananaNotifications.DisplayErrorNotification("<align=center><size=2><b>You are on an old version of Custom Cosmetics\nPlease check the watch page for more info</b></size></align>", 5f);
					Debug.Log("Incorrect Version");
					StringBuilder str = new StringBuilder();
					str.AppendLine("<color=red>==Wrong Version==</color>");
					str.AppendLine("");
					str.AppendLine("You are on an old version of Custom Cosmetics");
					str.AppendLine("Your Version: 1.2.5.0\nLatest Version: " + info[0] + "\n\nPlease download the latest version of the mod from the discord:\n" + info[1]);
					errorText = str;
					NetworkingBehaviours.assetCache.Clear();
					NetworkingBehaviours.nameAssetCache.Clear();
					correctVersion = false;
					MonkeWatch.Instance.UpdateScreen();
					str = null;
				}
				else
				{
					correctVersion = true;
				}
				info = null;
			}
			yield break;
		}

		private void GameInitialized(Scene scene, LoadSceneMode loadMode)
		{
			bool flag = scene.name == "GorillaTag";
			if (flag)
			{
				base.StartCoroutine(checkVersion());
				NetworkingBehaviours.currentTaggedMaterial.mat = null;
				NetworkingBehaviours.currentMaterial.mat = null;
				removeCosmetics = base.Config.Bind<bool>("Settings", "Remove Cosmetics", false, "Whether the mod should unequip normal cosmetics when equipping custom ones.");
				hat = base.Config.Bind<string>("Cosmetics", "Current Hat", "", "This is the current hat your using.");
				Lholdable = base.Config.Bind<string>("Cosmetics", "Current Left Holdable", "", "This is the current left holdable your using.");
				Rholdable = base.Config.Bind<string>("Cosmetics", "Current Right Holdable", "", "This is the current right holdable your using.");
				badge = base.Config.Bind<string>("Cosmetics", "Current Badge", "", "This is the current badge your using.");
				material = base.Config.Bind<string>("Cosmetics", "Current Material", "", "This is the current material your using.");
				taggedMaterial = base.Config.Bind<string>("Cosmetics", "Current Tagged Material", "", "This is the current tagged material your using.");
				bool flag2 = !Directory.Exists(cosmeticPath);
				if (flag2)
				{
					Directory.CreateDirectory(cosmeticPath);
				}
				bool flag3 = !Directory.Exists(cosmeticPath + "/Hats");
				if (flag3)
				{
					Directory.CreateDirectory(cosmeticPath + "/Hats");
				}
				bool flag4 = !Directory.Exists(cosmeticPath + "/Holdables");
				if (flag4)
				{
					Directory.CreateDirectory(cosmeticPath + "/Holdables");
				}
				bool flag5 = !Directory.Exists(cosmeticPath + "/Badges");
				if (flag5)
				{
					Directory.CreateDirectory(cosmeticPath + "/Badges");
				}
				bool flag6 = !Directory.Exists(cosmeticPath + "/Materials");
				if (flag6)
				{
					Directory.CreateDirectory(cosmeticPath + "/Materials");
				}
				ComponentUtils.AddComponent<Net>(this);
				Harmony harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.GUID);
				Type type = typeof(GorillaTagger).Assembly.GetType("VRRigCache");
				harmony.Patch(AccessTools.Method(type, "AddRigToGorillaParent", null, null), null, new HarmonyMethod(typeof(RigJoinPatch), "Patch", null), null, null, null);
				harmony.Patch(AccessTools.Method(type, "RemoveRigFromGorillaParent", null, null), new HarmonyMethod(typeof(RigLeavePatch), "Patch", null), null, null, null, null);
				LoadAssets();
			}
		}

		public async Task LoadAssets()
		{
			bool flag = !correctVersion;
			if (!flag)
			{
				string currentCosmeticLoading = "null";
				loadError = false;
				try
				{
					Debug.Log("Loading Custom Cosmetics");
					GameObject cosmeticsParent = new GameObject("CustomCosmetics");
					foreach (string hat in Directory.GetFiles(cosmeticPath + "/Hats/", "*.hat"))
					{
						currentCosmeticLoading = hat;
						AssetBundle assetBundle = await AssetLoader.LoadBundle(hat);
						AssetBundle hatbundle = assetBundle;
						assetBundle = null;
						GameObject temphat = hatbundle.LoadAsset<GameObject>("hat");
						temphat.transform.SetParent(cosmeticsParent.transform);
						hatbundle.Unload(false);
						NetworkingBehaviours.assetCache.TryAdd(Path.GetFileName(hat), temphat);
						string hatname = GetCosName(temphat, "Hat");
						NetworkingBehaviours.nameAssetCache.Add(hatname, temphat);

					}

					foreach (string holdable in Directory.GetFiles(cosmeticPath + "/Holdables/", "*.holdable"))
					{
						currentCosmeticLoading = holdable;
						AssetBundle assetBundle2 = await AssetLoader.LoadBundle(holdable);
						AssetBundle holdablebundle = assetBundle2;
						assetBundle2 = null;
						GameObject tempholdable = holdablebundle.LoadAsset<GameObject>("holdABLE");
						tempholdable.transform.SetParent(cosmeticsParent.transform);
						holdablebundle.Unload(false);
						NetworkingBehaviours.assetCache.TryAdd(Path.GetFileName(holdable), tempholdable);
						string holdablename = GetCosName(tempholdable, "Holdable");
						NetworkingBehaviours.nameAssetCache.Add(holdablename, tempholdable);
					}

					foreach (string badge in Directory.GetFiles(cosmeticPath + "/Badges/", "*.badge"))
					{
						currentCosmeticLoading = badge;
						AssetBundle assetBundle3 = await AssetLoader.LoadBundle(badge);
						AssetBundle badgebundle = assetBundle3;
						assetBundle3 = null;
						GameObject tempbadge = badgebundle.LoadAsset<GameObject>("badge");
						tempbadge.transform.SetParent(cosmeticsParent.transform);
						badgebundle.Unload(false);
						NetworkingBehaviours.assetCache.TryAdd(Path.GetFileName(badge), tempbadge);
						string badgename = GetCosName(tempbadge, "Badge");
						NetworkingBehaviours.nameAssetCache.Add(badgename, tempbadge);
					}

					foreach (string material in Directory.GetFiles(cosmeticPath + "/Materials/", "*.material"))
					{
						currentCosmeticLoading = material;
						AssetBundle assetBundle4 = await AssetLoader.LoadBundle(material);
						AssetBundle materialbundle = assetBundle4;
						assetBundle4 = null;
						GameObject tempmaterial = materialbundle.LoadAsset<GameObject>("material");
						tempmaterial.transform.SetParent(cosmeticsParent.transform);
						materialbundle.Unload(false);
						NetworkingBehaviours.assetCache.TryAdd(Path.GetFileName(material), tempmaterial);
						string matname = GetCosName(tempmaterial, "Material");
						NetworkingBehaviours.nameAssetCache.Add(matname, tempmaterial);
					}
					NetworkingBehaviours.defaultTaggedMaterial = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[2];
					string savedhat = hat.Value;
					string savedlholdable = Lholdable.Value;
					string savedrholdable = Rholdable.Value;
					string savedbadge = badge.Value;
					string savedmaterial = material.Value;
					string savedtagmaterial = taggedMaterial.Value;
					if (File.Exists(cosmeticPath + "/Hats/" + savedhat))
					{
						InfoLoader.GetInfo(savedhat, "Hat");
						LoadHat(cosmeticPath + "/Hats/" + savedhat);
					}
					if (File.Exists(cosmeticPath + "/Holdables/" + savedrholdable))
					{
						InfoLoader.GetInfo(savedrholdable, "Holdable");
						LoadHoldable(cosmeticPath + "/Holdables/" + savedrholdable, false);
					}
					if (File.Exists(cosmeticPath + "/Holdables/" + savedlholdable))
					{
						InfoLoader.GetInfo(savedlholdable, "Holdable");
						LoadHoldable(cosmeticPath + "/Holdables/" + savedlholdable, true);
					}
					if (File.Exists(cosmeticPath + "/Badges/" + savedbadge))
					{
						InfoLoader.GetInfo(savedbadge, "Badge");
						LoadBadge(cosmeticPath + "/Badges/" + savedbadge);
					}
					if (File.Exists(cosmeticPath + "/Materials/" + savedmaterial))
					{
						InfoLoader.GetInfo(savedmaterial, "Material");
						LoadMaterial(cosmeticPath + "/Materials/" + savedmaterial, 0);
					}
					if (File.Exists(cosmeticPath + "/Materials/" + savedtagmaterial))
					{
						InfoLoader.GetInfo(savedtagmaterial, "Material");
						LoadMaterial(cosmeticPath + "/Materials/" + savedtagmaterial, 2);
					}
					assetsLoaded = true;
					MonkeWatch.Instance.UpdateScreen();
					BananaNotifications.DisplayNotification("<align=center><size=2><b>Finished Loading Custom Cosmetics!\n Have Fun!</b></size></align>", new Color(0.424f, 0.086f, 0.839f, 1f), Color.white, 2f);
					Debug.Log("Finished Loading Custom Cosmetics");
				}
				catch (Exception ex)
				{
					brokenCosmetic = currentCosmeticLoading;
					Debug.Log("Issue when loading CustomCosmetics");
					StringBuilder str = new StringBuilder();
					str.AppendLine("<color=red>==Error When Loading==</color>");
					str.AppendLine("");
					str.AppendLine("There was an error when loading cosmetics.");
					str.AppendLine("You have a broken cosmetic installed, \nplease click enter to delete cosmetic " + Path.GetFileName(currentCosmeticLoading) + " and reload the mod");
					errorText = str;
					NetworkingBehaviours.assetCache.Clear();
					NetworkingBehaviours.nameAssetCache.Clear();
					Debug.LogError(ex);
					loadError = true;
					MonkeWatch.Instance.UpdateScreen();
					BananaNotifications.DisplayErrorNotification("<align=center><size=2><b>Error when loading Custom Cosmetics\n Please check the Cosmetics page on the watch for more info</b></size></align>", 5f);
				}
			}
		}

		public void LoadHoldable(string file, bool lHand)
		{
			bool flag = file == "DisableR";
			if (flag)
			{
				Destroy(NetworkingBehaviours.currentRHoldable);
				Rholdable.Value = "";
				ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
				ExtensionMethods.AddOrUpdate<object, object>(customProperties, "CustomRHoldable", "");
				PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
			}
			else
			{
				bool flag2 = file == "DisableL";
				if (flag2)
				{
					Destroy(NetworkingBehaviours.currentLHoldable);
					Lholdable.Value = "";
					ExitGames.Client.Photon.Hashtable customProperties2 = PhotonNetwork.LocalPlayer.CustomProperties;
					ExtensionMethods.AddOrUpdate<object, object>(customProperties2, "CustomLHoldable", "");
					PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties2, null, null);
				}
				else
				{
					GameObject gameObject;
					bool flag3 = !NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject);
					if (!flag3)
					{
						GameObject gameObject2;
						NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject2);
						GameObject gameObject3 = Instantiate<GameObject>(gameObject2);
						bool flag4 = gameObject3 != null;
						if (flag4)
						{
							GameObject gameObject4 = gameObject3;
							bool flag5 = !usingTextMethod;
							if (flag5)
							{
								foreach (Collider collider in gameObject4.GetComponentsInChildren<Collider>())
								{
									Destroy(collider);
								}
								bool flag6 = !lHand;
								if (flag6)
								{
									GameObject gameObject5 = Instantiate<GameObject>(holdableDes.rightHandObject);
									bool flag7 = holdableDes.behaviours.Count > 0;
									if (flag7)
									{
										foreach (CosmeticBehaviour cosmeticBehaviour in holdableDes.behaviours)
										{
											CustomBehaviour customBehaviour = cosmeticBehaviour.gameObject.AddComponent<CustomBehaviour>();
											customBehaviour.button = cosmeticBehaviour.button;
											foreach (GameObject gameObject6 in cosmeticBehaviour.objectsToToggle)
											{
												customBehaviour.objectsToToggle.Add(OVRExtensions.FindChildRecursive(gameObject5.transform, gameObject6.name).gameObject);
											}
										}
									}
									Destroy(NetworkingBehaviours.currentRHoldable);
									NetworkingBehaviours.currentRHoldable = gameObject5;
									Rholdable.Value = Path.GetFileName(file);
									ExitGames.Client.Photon.Hashtable customProperties3 = PhotonNetwork.LocalPlayer.CustomProperties;
									ExtensionMethods.AddOrUpdate<object, object>(customProperties3, "CustomRHoldable", holdableDes.Name);
									PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties3, null, null);
									gameObject5.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
								}
								else if (lHand)
								{
									GameObject gameObject7 = Instantiate<GameObject>(holdableDes.leftHandObject);
									bool flag8 = holdableDes.behaviours.Count > 0;
									if (flag8)
									{
										foreach (CosmeticBehaviour cosmeticBehaviour2 in holdableDes.behaviours)
										{
											CustomBehaviour customBehaviour2 = cosmeticBehaviour2.gameObject.AddComponent<CustomBehaviour>();
											customBehaviour2.button = cosmeticBehaviour2.button;
											foreach (GameObject gameObject8 in cosmeticBehaviour2.objectsToToggle)
											{
												customBehaviour2.objectsToToggle.Add(OVRExtensions.FindChildRecursive(gameObject7.transform, gameObject8.name).gameObject);
											}
										}
									}
									Destroy(NetworkingBehaviours.currentLHoldable);
									NetworkingBehaviours.currentLHoldable = gameObject7;
									Lholdable.Value = Path.GetFileName(file);
									ExitGames.Client.Photon.Hashtable customProperties4 = PhotonNetwork.LocalPlayer.CustomProperties;
									ExtensionMethods.AddOrUpdate<object, object>(customProperties4, "CustomLHoldable", holdableDes.Name);
									PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties4, null, null);
									gameObject7.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/").transform, false);
								}
								Destroy(gameObject4);
							}
							else
							{
								foreach (Collider collider2 in gameObject4.GetComponentsInChildren<Collider>())
								{
									Destroy(collider2);
								}
								bool flag9 = !lHand;
								if (flag9)
								{
									Destroy(NetworkingBehaviours.currentRHoldable);
									NetworkingBehaviours.currentRHoldable = gameObject4;
									Rholdable.Value = Path.GetFileName(file);
									ExitGames.Client.Photon.Hashtable customProperties5 = PhotonNetwork.LocalPlayer.CustomProperties;
									ExtensionMethods.AddOrUpdate<object, object>(customProperties5, "CustomRHoldable", cosmeticName);
									PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties5, null, null);
									gameObject4.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
								}
								else if (lHand)
								{
									Destroy(NetworkingBehaviours.currentLHoldable);
									NetworkingBehaviours.currentLHoldable = gameObject4;
									Lholdable.Value = Path.GetFileName(file);
									ExitGames.Client.Photon.Hashtable customProperties6 = PhotonNetwork.LocalPlayer.CustomProperties;
									ExtensionMethods.AddOrUpdate<object, object>(customProperties6, "CustomLHoldable", cosmeticName);
									PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties6, null, null);
									gameObject4.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/").transform, false);
								}
							}
						}
					}
				}
			}
		}

		public void LoadHat(string file)
		{
			bool flag = file == "Disable";
			if (flag)
			{
				Destroy(NetworkingBehaviours.currentHat);
				hat.Value = "";
				ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
				ExtensionMethods.AddOrUpdate<object, object>(customProperties, "CustomHat", "");
				PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
			}
			else
			{
				GameObject gameObject;
				bool flag2 = !NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject);
				if (!flag2)
				{
					bool value = removeCosmetics.Value;
					if (value)
					{
						RemoveItem(CosmeticsController.CosmeticCategory.Hat, CosmeticsController.CosmeticSlots.Hat);
					}
					GameObject gameObject2;
					NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject2);
					GameObject gameObject3 = Instantiate<GameObject>(gameObject2);
					hat.Value = Path.GetFileName(file);
					bool flag3 = !usingTextMethod;
					if (flag3)
					{
						ExitGames.Client.Photon.Hashtable customProperties2 = PhotonNetwork.LocalPlayer.CustomProperties;
						ExtensionMethods.AddOrUpdate<object, object>(customProperties2, "CustomHat", hatDes.Name);
						PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties2, null, null);
					}
					else
					{
						ExitGames.Client.Photon.Hashtable customProperties3 = PhotonNetwork.LocalPlayer.CustomProperties;
						ExtensionMethods.AddOrUpdate<object, object>(customProperties3, "CustomHat", cosmeticName);
						PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties3, null, null);
					}
					bool flag4 = gameObject3 != null;
					if (flag4)
					{
						GameObject gameObject4 = gameObject3;
						bool flag5 = !usingTextMethod;
						if (flag5)
						{
							foreach (Collider collider in gameObject4.GetComponentsInChildren<Collider>())
							{
								Destroy(collider);
							}
							bool flag6 = hatDes.behaviours.Count > 0;
							if (flag6)
							{
								foreach (CosmeticBehaviour cosmeticBehaviour in hatDes.behaviours)
								{
									CustomBehaviour customBehaviour = gameObject4.AddComponent<CustomBehaviour>();
									customBehaviour.button = cosmeticBehaviour.button;
									foreach (GameObject gameObject5 in cosmeticBehaviour.objectsToToggle)
									{
										customBehaviour.objectsToToggle.Add(OVRExtensions.FindChildRecursive(gameObject4.transform, gameObject5.name).gameObject);
									}
								}
							}
						}
						else
						{
							foreach (Collider collider2 in gameObject4.GetComponentsInChildren<Collider>())
							{
								Destroy(collider2);
							}
						}
						bool flag7 = NetworkingBehaviours.currentHat != null;
						if (flag7)
						{
							Destroy(NetworkingBehaviours.currentHat);
						}
						NetworkingBehaviours.currentHat = gameObject4;
						gameObject4.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/head/").transform, false);
					}
				}
			}
		}

		public void LoadBadge(string file)
		{
			bool flag = file == "Disable";
			if (flag)
			{
				Destroy(NetworkingBehaviours.currentBadge);
				badge.Value = "";
				ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
				ExtensionMethods.AddOrUpdate<object, object>(customProperties, "CustomBadge", "");
				PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
			}
			GameObject gameObject;
			bool flag2 = !NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject);
			if (!flag2)
			{
				bool value = removeCosmetics.Value;
				if (value)
				{
					RemoveItem(CosmeticsController.CosmeticCategory.Badge, CosmeticsController.CosmeticSlots.Badge);
				}
				GameObject gameObject2;
				NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject2);
				GameObject gameObject3 = Instantiate<GameObject>(gameObject2);
				badge.Value = Path.GetFileName(file);
				bool flag3 = !usingTextMethod;
				if (flag3)
				{
					ExitGames.Client.Photon.Hashtable customProperties2 = PhotonNetwork.LocalPlayer.CustomProperties;
					ExtensionMethods.AddOrUpdate<object, object>(customProperties2, "CustomBadge", badgeDes.Name);
					PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties2, null, null);
				}
				else
				{
					ExitGames.Client.Photon.Hashtable customProperties3 = PhotonNetwork.LocalPlayer.CustomProperties;
					ExtensionMethods.AddOrUpdate<object, object>(customProperties3, "CustomBadge", cosmeticName);
					PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties3, null, null);
				}
				bool flag4 = gameObject3 != null;
				if (flag4)
				{
					GameObject gameObject4 = gameObject3;
					foreach (Collider collider in gameObject4.GetComponentsInChildren<Collider>())
					{
						Destroy(collider);
					}
					Destroy(NetworkingBehaviours.currentBadge);
					NetworkingBehaviours.currentBadge = gameObject4;
					gameObject4.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/").transform, false);
				}
			}
		}

		public void CheckPlayer(NetPlayer player, VRRig playerRig)
		{
			try
			{
				bool flag = !playerRig.isLocal;
				if (flag)
				{
                    ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false).CustomProperties;
					Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false);
					normalplayers.Add(playerRig, player2);
					Debug.Log(player.NickName + " entered the room");
					object obj;
					object obj2;
					object obj3;
					object obj4;
					object obj5;
					object obj6;
					bool flag2 = customProperties.TryGetValue("CustomHat", out obj) || customProperties.TryGetValue("CustomLHoldable", out obj2) || customProperties.TryGetValue("CustomRHoldable", out obj3) || customProperties.TryGetValue("CustomBadge", out obj4) || customProperties.TryGetValue("CustomMaterial", out obj5) || customProperties.TryGetValue("CustomTagMaterial", out obj6);
					if (flag2)
					{
						NetworkingBehaviours.cosmeticsplayers.Add(playerRig, player2);
						SetCosmetics(playerRig, customProperties, player2);
					}
				}
				else
				{
					ExitGames.Client.Photon.Hashtable customProperties2 = PhotonNetwork.LocalPlayer.CustomProperties;
					ExtensionMethods.AddOrUpdate<object, object>(customProperties2, "Colour", GorillaTagger.Instance.offlineVRRig.playerColor.ToString());
					PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties2, null, null);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		public void RemovePlayer(NetPlayer p, VRRig r)
		{
			try
			{
				bool flag = !r.isLocal;
				if (flag)
				{
					Player player = normalplayers[r];
					ExitGames.Client.Photon.Hashtable customProperties = player.CustomProperties;
					object obj;
					object obj2;
					object obj3;
					object obj4;
					object obj5;
					bool flag2 = customProperties.TryGetValue("CustomHat", out obj) || customProperties.TryGetValue("CustomLHoldable", out obj2) || customProperties.TryGetValue("CustomRHoldable", out obj3) || customProperties.TryGetValue("CustomMaterial", out obj4) || customProperties.TryGetValue("CustomTagMaterial", out obj5);
					if (flag2)
					{
						RemoveCosmetics(customProperties, r, player);
					}
					normalplayers.Remove(r);
				}
				else
				{
					EnableMaterial();
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		public void RemoveCosmetics(ExitGames.Client.Photon.Hashtable props, VRRig r, Player player)
		{
			object obj;
			object obj2;
			object obj3;
			object obj4;
			object obj5;
			object obj6;
			bool flag = props.TryGetValue("CustomHat", out obj) || props.TryGetValue("CustomLHoldable", out obj2) || props.TryGetValue("CustomRHoldable", out obj3) || props.TryGetValue("CustomBadge", out obj4) || props.TryGetValue("CustomMaterial", out obj5) || props.TryGetValue("CustomTagMaterial", out obj6);
			if (flag)
			{
				bool flag2 = NetworkingBehaviours.networkHats.ContainsKey(player);
				if (flag2)
				{
					Destroy(NetworkingBehaviours.networkHats[player]);
					NetworkingBehaviours.networkHats.Remove(player);
				}
				bool flag3 = NetworkingBehaviours.networkLHoldables.ContainsKey(player);
				if (flag3)
				{
					Destroy(NetworkingBehaviours.networkLHoldables[player]);
					NetworkingBehaviours.networkLHoldables.Remove(player);
				}
				bool flag4 = NetworkingBehaviours.networkRHoldables.ContainsKey(player);
				if (flag4)
				{
					Destroy(NetworkingBehaviours.networkRHoldables[player]);
					NetworkingBehaviours.networkRHoldables.Remove(player);
				}
				bool flag5 = NetworkingBehaviours.networkBadges.ContainsKey(player);
				if (flag5)
				{
					Destroy(NetworkingBehaviours.networkBadges[player]);
					NetworkingBehaviours.networkBadges.Remove(player);
				}
				r.materialsToChangeTo[0] = r.myDefaultSkinMaterialInstance;
				r.materialsToChangeTo[2] = NetworkingBehaviours.defaultTaggedMaterial;
				Material[] sharedMaterials = r.mainSkin.sharedMaterials;
				sharedMaterials[0] = r.materialsToChangeTo[r.setMatIndex];
				sharedMaterials[1] = r.defaultSkin.chestMaterial;
				r.mainSkin.sharedMaterials = sharedMaterials;
				NetworkingBehaviours.cosmeticsplayers.Remove(r);
			}
		}

		public void SetCosmetics(VRRig playerRig, ExitGames.Client.Photon.Hashtable props, Player playerr)
		{
			bool flag = playerRig != null;
			if (flag)
			{
				object obj;
				bool flag2 = props.TryGetValue("CustomHat", out obj);
				if (flag2)
				{
					GameObject gameObject;
					bool flag3 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj.ToString(), out gameObject);
					if (flag3)
					{
						NetworkingBehaviours.LoadNetworkHat(obj.ToString(), playerRig, playerr);
					}
				}
				object obj2;
				bool flag4 = props.TryGetValue("CustomRHoldable", out obj2);
				if (flag4)
				{
					GameObject gameObject2;
					bool flag5 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj2.ToString(), out gameObject2);
					if (flag5)
					{
						NetworkingBehaviours.LoadNetworkHoldable(obj2.ToString(), playerRig, playerr, false);
					}
				}
				object obj3;
				bool flag6 = props.TryGetValue("CustomLHoldable", out obj3);
				if (flag6)
				{
					GameObject gameObject3;
					bool flag7 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj3.ToString(), out gameObject3);
					if (flag7)
					{
						NetworkingBehaviours.LoadNetworkHoldable(obj3.ToString(), playerRig, playerr, true);
					}
				}
				object obj4;
				if (props.TryGetValue("CustomBadge", out obj4))
				{
					GameObject gameObject4;
					bool flag9 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj4.ToString(), out gameObject4);
					if (flag9)
					{
						NetworkingBehaviours.LoadNetworkBadge(obj4.ToString(), playerRig, playerr);
					}
				}
				object obj5;
				bool flag10 = props.TryGetValue("CustomMaterial", out obj5);
				if (flag10)
				{
					GameObject gameObject5;
					bool flag11 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj5.ToString(), out gameObject5);
					if (flag11)
					{
						object obj6;
						props.TryGetValue("Colour", out obj6);
						Color color = ColorExtensions.parseColor(obj6.ToString());
						NetworkingBehaviours.LoadNetworkMaterial(obj5.ToString(), 0, playerRig, playerr, color);
					}
				}
				object obj7;
				bool flag12 = props.TryGetValue("CustomTagMaterial", out obj7);
				if (flag12)
				{
					GameObject gameObject6;
					bool flag13 = NetworkingBehaviours.nameAssetCache.TryGetValue(obj7.ToString(), out gameObject6);
					if (flag13)
					{
						object obj8;
						props.TryGetValue("Colour", out obj8);
						Color color2 = ColorExtensions.parseColor(obj8.ToString());
						NetworkingBehaviours.LoadNetworkMaterial(obj7.ToString(), 2, playerRig, playerr, color2);
					}
				}
			}
		}

		public void CheckItems()
		{
			bool value = removeCosmetics.Value;
			if (value)
			{
				CosmeticsController.CosmeticItem[] items = CosmeticsController.instance.currentWornSet.items;
				for (int i = 0; i < items.Length; i++)
				{
					bool flag = items[i].itemCategory == CosmeticsController.CosmeticCategory.Hat;
					if (flag)
					{
						LoadHat("Disable");
					}
					bool flag2 = items[i].itemCategory == CosmeticsController.CosmeticCategory.Badge;
					if (flag2)
					{
						LoadBadge("Disable");
					}
				}
			}
		}

		public static void RemoveItem(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			try
			{
				bool flag = false;
				CosmeticsController.CosmeticItem nullItem = CosmeticsController.instance.nullItem;
				CosmeticsController.CosmeticItem[] array = CosmeticsController.instance.currentWornSet.items;
				for (int i = 0; i < array.Length; i++)
				{
					bool flag2 = array[i].itemCategory == category && !array[i].isNullItem;
					if (flag2)
					{
						flag = true;
						array[i] = nullItem;
					}
				}
				array = CosmeticsController.instance.tryOnSet.items;
				for (int j = 0; j < array.Length; j++)
				{
					bool flag3 = array[j].itemCategory == category && !array[j].isNullItem;
					if (flag3)
					{
						flag = true;
						array[j] = nullItem;
					}
				}
				bool flag4 = flag;
				if (flag4)
				{
					CosmeticsController.instance.UpdateShoppingCart();
					CosmeticsController.instance.UpdateWornCosmetics(true);
					PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), nullItem.itemName);
					PlayerPrefs.Save();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Failed to remove game cosmetic\n",
					ex.GetType().Name,
					" (",
					ex.Message,
					")"
				}));
			}
		}

		public void LoadMaterial(string file, int materialIndex)
		{
			bool flag = file == "Disable";
			if (flag)
			{
				bool flag2 = materialIndex == 0;
				if (flag2)
				{
					material.Value = "";
					NetworkingBehaviours.currentMaterial.mat = null;
					ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
					ExtensionMethods.AddOrUpdate<object, object>(customProperties, "CustomMaterial", "");
					PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
					VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
					offlineVRRig.materialsToChangeTo[0] = offlineVRRig.myDefaultSkinMaterialInstance;
					Material[] sharedMaterials = offlineVRRig.mainSkin.sharedMaterials;
					sharedMaterials[0] = offlineVRRig.materialsToChangeTo[offlineVRRig.setMatIndex];
					sharedMaterials[1] = offlineVRRig.defaultSkin.chestMaterial;
					offlineVRRig.mainSkin.sharedMaterials = sharedMaterials;
				}
				else
				{
					bool flag3 = materialIndex == 2;
					if (flag3)
					{
						taggedMaterial.Value = "";
						NetworkingBehaviours.currentTaggedMaterial.mat = null;
						ExitGames.Client.Photon.Hashtable customProperties2 = PhotonNetwork.LocalPlayer.CustomProperties;
						ExtensionMethods.AddOrUpdate<object, object>(customProperties2, "CustomTagMaterial", "");
						PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties2, null, null);
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.materialsToChangeTo[2] = NetworkingBehaviours.defaultTaggedMaterial;
						Material[] sharedMaterials2 = offlineVRRig2.mainSkin.sharedMaterials;
						sharedMaterials2[0] = offlineVRRig2.materialsToChangeTo[offlineVRRig2.setMatIndex];
						sharedMaterials2[1] = offlineVRRig2.defaultSkin.chestMaterial;
						offlineVRRig2.mainSkin.sharedMaterials = sharedMaterials2;
					}
				}
			}
			GameObject gameObject;
			bool flag4 = !NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject);
			if (!flag4)
			{
				bool flag5 = materialIndex == 0;
				if (flag5)
				{
					material.Value = Path.GetFileName(file);
				}
				else
				{
					bool flag6 = materialIndex == 2;
					if (flag6)
					{
						taggedMaterial.Value = Path.GetFileName(file);
					}
				}
				GameObject gameObject2;
				NetworkingBehaviours.assetCache.TryGetValue(Path.GetFileName(file), out gameObject2);
				GameObject gameObject3 = Instantiate(gameObject2);
				RemoveItem(CosmeticsController.CosmeticCategory.Fur, CosmeticsController.CosmeticSlots.Fur);
				bool flag7 = gameObject3 != null;
				if (flag7)
				{
					GameObject gameObject4 = gameObject3;
					try
					{
						bool flag8 = materialIndex == 0;
						if (flag8)
						{
							VRRig offlineVRRig3 = GorillaTagger.Instance.offlineVRRig;
							NetworkingBehaviours.currentMaterial.mat = gameObject4.GetComponent<MeshRenderer>().material;
							bool flag9 = usingTextMethod;
							if (flag9)
							{
								NetworkingBehaviours.currentMaterial.customColours = materialCustomColours;
							}
							else
							{
								NetworkingBehaviours.currentMaterial.customColours = matDes.customColors;
							}
							bool customColours = NetworkingBehaviours.currentMaterial.customColours;
							if (customColours)
							{
								NetworkingBehaviours.currentMaterial.mat.color = offlineVRRig3.playerColor;
							}
							ExitGames.Client.Photon.Hashtable customProperties3 = PhotonNetwork.LocalPlayer.CustomProperties;
							ExtensionMethods.AddOrUpdate<object, object>(customProperties3, "CustomMaterial", matDes.Name);
							PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties3, null, null);
							offlineVRRig3.materialsToChangeTo[materialIndex] = NetworkingBehaviours.currentMaterial.mat;
							Material[] sharedMaterials3 = offlineVRRig3.mainSkin.sharedMaterials;
							sharedMaterials3[0] = offlineVRRig3.materialsToChangeTo[offlineVRRig3.setMatIndex];
							sharedMaterials3[1] = offlineVRRig3.defaultSkin.chestMaterial;
							offlineVRRig3.mainSkin.sharedMaterials = sharedMaterials3;
						}
						else
						{
							bool flag10 = materialIndex == 2;
							if (flag10)
							{
								NetworkingBehaviours.currentTaggedMaterial.mat = gameObject4.GetComponent<MeshRenderer>().material;
								bool flag11 = usingTextMethod;
								if (flag11)
								{
									NetworkingBehaviours.currentTaggedMaterial.customColours = materialCustomColours;
								}
								else
								{
									NetworkingBehaviours.currentTaggedMaterial.customColours = matDes.customColors;
								}
								VRRig offlineVRRig4 = GorillaTagger.Instance.offlineVRRig;
								bool customColours2 = NetworkingBehaviours.currentTaggedMaterial.customColours;
								if (customColours2)
								{
									NetworkingBehaviours.currentTaggedMaterial.mat.color = new Color(1f, 0.4f, 0f);
								}
								ExitGames.Client.Photon.Hashtable customProperties4 = PhotonNetwork.LocalPlayer.CustomProperties;
								ExtensionMethods.AddOrUpdate<object, object>(customProperties4, "CustomTagMaterial", matDes.Name);
								PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties4, null, null);
								offlineVRRig4.materialsToChangeTo[materialIndex] = NetworkingBehaviours.currentTaggedMaterial.mat;
								Material[] sharedMaterials4 = offlineVRRig4.mainSkin.sharedMaterials;
								sharedMaterials4[0] = offlineVRRig4.materialsToChangeTo[offlineVRRig4.setMatIndex];
								sharedMaterials4[1] = offlineVRRig4.defaultSkin.chestMaterial;
								offlineVRRig4.mainSkin.sharedMaterials = sharedMaterials4;
							}
							else
							{
								VRRig offlineVRRig5 = GorillaTagger.Instance.offlineVRRig;
								offlineVRRig5.materialsToChangeTo[materialIndex] = NetworkingBehaviours.currentTaggedMaterial.mat;
							}
						}
						Destroy(gameObject4);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
		}

		public string GetCosName(GameObject obj, string mode)
		{
			string text = "Empty";
			if (obj.TryGetComponent<Text>(out Text text2))
			{
				usingTextMethod = true;
				string[] array = text2.text.Split("$", StringSplitOptions.None);
				if (!(mode == "Material"))
				{
					if (!(mode == "Holdable"))
					{
						if (!(mode == "Badge"))
						{
							if (mode == "Hat")
							{
								text = array[0];
							}
						}
						else
						{
							text = array[0];
						}
					}
					else
					{
						text = array[0];
					}
				}
				else
				{
					text = array[0];
				}
			}
			else
			{
				usingTextMethod = false;
				if (!(mode == "Material"))
				{
					if (!(mode == "Holdable"))
					{
						if (!(mode == "Hat"))
						{
							if (mode == "Badge")
							{
								text = obj.GetComponent<BadgeDescriptor>().Name;
							}
						}
						else
						{
							text = obj.GetComponent<HatDescriptor>().Name;
						}
					}
					else
					{
						text = obj.GetComponent<HoldableDescriptor>().Name;
					}
				}
				else
				{
					text = obj.GetComponent<MaterialDescriptor>().Name;
				}
			}
			return text;
		}

		public void EnableMaterial()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			bool customColours = NetworkingBehaviours.currentMaterial.customColours;
			if (customColours)
			{
				NetworkingBehaviours.currentMaterial.mat.color = offlineVRRig.playerColor;
			}
			offlineVRRig.materialsToChangeTo[0] = NetworkingBehaviours.currentMaterial.mat;
			Material[] sharedMaterials = offlineVRRig.mainSkin.sharedMaterials;
			sharedMaterials[0] = offlineVRRig.materialsToChangeTo[offlineVRRig.setMatIndex];
			sharedMaterials[1] = offlineVRRig.defaultSkin.chestMaterial;
			offlineVRRig.mainSkin.sharedMaterials = sharedMaterials;
			bool flag = NetworkingBehaviours.currentTaggedMaterial.customColours && NetworkingBehaviours.currentTaggedMaterial.mat != null;
			if (flag)
			{
				NetworkingBehaviours.currentTaggedMaterial.mat.color = new Color(1f, 0.4f, 0f);
			}
		}

		public void UpdateColour(Color colour)
		{
			ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			ExtensionMethods.AddOrUpdate<object, object>(customProperties, "Colour", colour.ToString());
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
			bool flag = NetworkingBehaviours.currentMaterial.mat != null;
			if (flag)
			{
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.materialsToChangeTo[0] = NetworkingBehaviours.currentMaterial.mat;
				bool customColours = NetworkingBehaviours.currentMaterial.customColours;
				if (customColours)
				{
					NetworkingBehaviours.currentMaterial.mat.color = colour;
				}
				Material[] sharedMaterials = offlineVRRig.mainSkin.sharedMaterials;
				sharedMaterials[0] = offlineVRRig.materialsToChangeTo[offlineVRRig.setMatIndex];
				sharedMaterials[1] = offlineVRRig.defaultSkin.chestMaterial;
				offlineVRRig.mainSkin.sharedMaterials = sharedMaterials;
			}
		}

		public static Plugin instance;

		public string cosmeticPath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "Cosmetics");

		public ConfigEntry<string> hat;
		public ConfigEntry<string> Lholdable;
		public ConfigEntry<string> Rholdable;
		public ConfigEntry<string> badge;
		public ConfigEntry<string> material;
		public ConfigEntry<string> taggedMaterial;
		public ConfigEntry<bool> removeCosmetics;

		private Dictionary<VRRig, Player> normalplayers = new Dictionary<VRRig, Player>();

		public int prevMatIndex;

		public bool assetsLoaded = false;

		public bool loadError = false;

		public StringBuilder errorText;
		public string brokenCosmetic;
		public bool correctVersion = true;

		public string cosmeticName;
		public string cosmeticAuthor;
		public string cosmeticDescription;
		public string currentCosmeticFile;

		public bool leftHand;

		public bool materialCustomColours;

		public bool usingTextMethod;


		public MaterialDescriptor matDes;
		public BadgeDescriptor badgeDes;
		public HatDescriptor hatDes;
		public HoldableDescriptor holdableDes;
	}
}
