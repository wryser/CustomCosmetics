#define RELEASE
using BepInEx;
using BepInEx.Configuration;
using GorillaLocomotion;
using GorillaTag.Reactions;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using GorillaNetworking;
using HarmonyLib;
using ExitGames.Client.Photon;
using CustomCosmetics.Patches;
using GorillaNetworking.Store;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System.Reflection;
using Unity.Mathematics;

namespace CustomCosmetics
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        GameObject currentRHoldable;
        GameObject currentLHoldable;
        GameObject currentHat;
        GameObject currentBadge;
        struct customMaterial
        {
            public Material mat;
            public bool customColours;
        }
        customMaterial currentMaterial = new customMaterial();
        customMaterial currentTaggedMaterial = new customMaterial();
        Material defaultTaggedMaterial;
        public string cosmeticPath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "Cosmetics");
        public ConfigEntry<string> hat;
        public ConfigEntry<string> Lholdable;
        public ConfigEntry<string> Rholdable;
        public ConfigEntry<string> badge;
        public ConfigEntry<string> material;
        public ConfigEntry<string> taggedMaterial;
        public ConfigEntry<bool> removeCosmetics;
        Dictionary<Photon.Realtime.Player, GameObject> networkHats = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkRHoldables = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkLHoldables = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkBadges = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<VRRig, Photon.Realtime.Player> cosmeticsplayers = new Dictionary<VRRig, Photon.Realtime.Player>();
        Dictionary<VRRig, Photon.Realtime.Player> normalplayers = new Dictionary<VRRig, Photon.Realtime.Player>();
        Dictionary<string, GameObject> assetCache = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> nameAssetCache = new Dictionary<string, GameObject>();
        public int prevMatIndex;
        public UnityEvent cosmeticsLoaded = new UnityEvent();
        public bool assetsLoaded = false;

        // General Cosmetic Info Values
        public string cosmeticName;
        public string cosmeticAuthor;
        public string cosmeticDescription;
        public string currentCosmeticFile;

        // Holdable Cosmetic Values
        public bool leftHand;

        // Material Cosmetic Values
        public bool materialCustomColours;

        // Old Method Check
        public bool usingTextMethod;

        // New Exporters
        public MaterialDescriptor matDes;
        public BadgeDescriptor badgeDes;
        public HatDescriptor hatDes;
        public HoldableDescriptor holdableDes;

        void Awake()
        {
            SceneManager.sceneLoaded += GameInitialized;
            instance = this;
        }

        void GameInitialized(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "GorillaTag")
            {
                /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
                currentTaggedMaterial.mat = null;
                currentMaterial.mat = null;
                removeCosmetics = Config.Bind("Settings", "Remove Cosmetics", false, "Whether the mod should unequip normal cosmetics when equipping custom ones.");
                hat = Config.Bind("Cosmetics", "Current Hat", "", "This is the current hat your using.");
                Lholdable = Config.Bind("Cosmetics", "Current Left Holdable", "", "This is the current left holdable your using.");
                Rholdable = Config.Bind("Cosmetics", "Current Right Holdable", "", "This is the current right holdable your using.");
                badge = Config.Bind("Cosmetics", "Current Badge", "", "This is the current badge your using.");
                material = Config.Bind("Cosmetics", "Current Material", "", "This is the current material your using.");
                taggedMaterial = Config.Bind("Cosmetics", "Current Tagged Material", "", "This is the current tagged material your using.");
                if (!Directory.Exists(cosmeticPath))
                {
                    Directory.CreateDirectory(cosmeticPath);
                }
                if (!Directory.Exists(cosmeticPath + "/Hats"))
                {
                    Directory.CreateDirectory(cosmeticPath + "/Hats");
                }
                if (!Directory.Exists(cosmeticPath + "/Holdables"))
                {
                    Directory.CreateDirectory(cosmeticPath + "/Holdables");
                }
                if (!Directory.Exists(cosmeticPath + "/Badges"))
                {
                    Directory.CreateDirectory(cosmeticPath + "/Badges");
                }
                if (!Directory.Exists(cosmeticPath + "/Materials"))
                {
                    Directory.CreateDirectory(cosmeticPath + "/Materials");
                }
                this.AddComponent<Net>();
                Harmony harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.GUID);
                Type rigCache = typeof(GorillaTagger).Assembly.GetType("VRRigCache");
                harmony.Patch(AccessTools.Method(rigCache, "AddRigToGorillaParent"), postfix: new HarmonyMethod(typeof(RigJoinPatch), nameof(RigJoinPatch.Patch)));
                harmony.Patch(AccessTools.Method(rigCache, "RemoveRigFromGorillaParent"), prefix: new HarmonyMethod(typeof(RigLeavePatch), nameof(RigLeavePatch.Patch)));
                GorillaTagger.Instance.offlineVRRig.OnColorChanged += UpdateColour;
                LoadAssets();
            }
        }

        public async Task LoadAssets()
        {
            try
            {
                Debug.Log("Loading Custom Cosmetics");
                GameObject cosmeticsParent = new GameObject("CustomCosmetics");
                foreach (string hat in Directory.GetFiles(cosmeticPath + "/Hats/"))
                {
                    AssetBundle hatbundle = await LoadBundle(hat);
                    GameObject temphat = Instantiate(hatbundle.LoadAsset<GameObject>("hat"));
                    temphat.transform.SetParent(cosmeticsParent.transform);
                    hatbundle.Unload(false);
                    assetCache.Add(Path.GetFileName(hat), temphat);
                    string hatname = GetCosName(temphat, "Hat");
                    nameAssetCache.Add(hatname, temphat);
                }
                foreach (string holdable in Directory.GetFiles(cosmeticPath + "/Holdables/"))
                {
                    AssetBundle holdablebundle = await LoadBundle(holdable);
                    GameObject tempholdable = Instantiate(holdablebundle.LoadAsset<GameObject>("holdABLE"));
                    tempholdable.transform.SetParent(cosmeticsParent.transform);
                    holdablebundle.Unload(false);
                    assetCache.Add(Path.GetFileName(holdable), tempholdable);
                    string holdablename = GetCosName(tempholdable, "Holdable");
                    nameAssetCache.Add(holdablename, tempholdable);
                }
                foreach (string badge in Directory.GetFiles(cosmeticPath + "/Badges/"))
                {
                    AssetBundle badgebundle = await LoadBundle(badge);
                    GameObject tempbadge = Instantiate(badgebundle.LoadAsset<GameObject>("badge"));
                    tempbadge.transform.SetParent(cosmeticsParent.transform);
                    badgebundle.Unload(false);
                    assetCache.Add(Path.GetFileName(badge), tempbadge);
                    string badgename = GetCosName(tempbadge, "Badge");
                    nameAssetCache.Add(badgename, tempbadge);
                }
                foreach (string material in Directory.GetFiles(cosmeticPath + "/Materials/"))
                {
                    AssetBundle materialbundle = await LoadBundle(material);
                    GameObject tempmaterial = Instantiate(materialbundle.LoadAsset<GameObject>("material"));
                    tempmaterial.transform.SetParent(cosmeticsParent.transform);
                    materialbundle.Unload(false);
                    assetCache.Add(Path.GetFileName(material), tempmaterial);
                    string matname = GetCosName(tempmaterial, "Material");
                    nameAssetCache.Add(matname, tempmaterial);
                }
                defaultTaggedMaterial = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[2];
                string savedhat = hat.Value;
                string savedlholdable = Lholdable.Value;
                string savedrholdable = Rholdable.Value;
                string savedbadge = badge.Value;
                string savedmaterial = material.Value;
                string savedtagmaterial = taggedMaterial.Value;
                if (savedhat != "")
                {
                    GetInfo(savedhat, "Hat");
                    LoadHat(cosmeticPath + "/Hats/" + savedhat);
                }
                if (savedrholdable != "")
                {
                    GetInfo(savedrholdable, "Holdable");
                    LoadHoldable(cosmeticPath + "/Holdables/" + savedrholdable, false);
                }
                if (savedlholdable != "")
                {
                    GetInfo(savedlholdable, "Holdable");
                    LoadHoldable(cosmeticPath + "/Holdables/" + savedlholdable, true);
                }
                if (savedbadge != "")
                {
                    GetInfo(savedbadge, "Badge");
                    LoadBadge(cosmeticPath + "/Badges/" + savedbadge);
                }
                if (savedmaterial != "")
                {
                    GetInfo(savedmaterial, "Material");
                    LoadMaterial(cosmeticPath + "/Materials/" + savedmaterial, 0);
                }
                if (savedtagmaterial != "")
                {
                    GetInfo(savedtagmaterial, "Material");
                    LoadMaterial(cosmeticPath + "/Materials/" + savedtagmaterial, 2);
                }
                cosmeticsLoaded.Invoke();
                assetsLoaded = true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            Debug.Log("Finished Loading Custom Cosmetics");
        }

        public void LoadHoldable(string file, bool lHand)
        {
            if (file == "DisableR")
            {
                Destroy(currentRHoldable);
                Rholdable.Value = "";
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomRHoldable", "");
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                return;
            }
            else if (file == "DisableL")
            {
                Destroy(currentLHoldable);
                Lholdable.Value = "";
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomLHoldable", "");
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                return;
            }

            if(!assetCache.TryGetValue(Path.GetFileName(file), out var empty))
            {
                return;
            }

            GameObject asset;
            assetCache.TryGetValue(Path.GetFileName(file), out asset);
            GameObject prefab = Instantiate(asset);
            if (prefab != null)
            {
                var parentAsset = prefab;
                if (!usingTextMethod)
                {
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                    if (!lHand)
                    {
                        GameObject rHoldable = Instantiate(holdableDes.rightHandObject);
                        if (holdableDes.behaviours.Count > 0)
                        {
                            foreach (CosmeticBehaviour behaviour in holdableDes.behaviours)
                            {
                                CustomBehaviour cbehaviour = behaviour.gameObject.AddComponent<CustomBehaviour>();
                                cbehaviour.button = behaviour.button;
                                foreach (GameObject o in behaviour.objectsToToggle)
                                {
                                    cbehaviour.objectsToToggle.Add(rHoldable.transform.FindChildRecursive(o.name).gameObject);
                                }
                            }
                        }
                        Destroy(currentRHoldable);
                        currentRHoldable = rHoldable;
                        Rholdable.Value = Path.GetFileName(file);
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomRHoldable", holdableDes.Name);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        rHoldable.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
                    }
                    else if (lHand)
                    {
                        GameObject lHoldable = Instantiate(holdableDes.leftHandObject);
                        if (holdableDes.behaviours.Count > 0)
                        {
                            foreach (CosmeticBehaviour behaviour in holdableDes.behaviours)
                            {
                                CustomBehaviour cbehaviour = behaviour.gameObject.AddComponent<CustomBehaviour>();
                                cbehaviour.button = behaviour.button;
                                foreach (GameObject o in behaviour.objectsToToggle)
                                {
                                    cbehaviour.objectsToToggle.Add(lHoldable.transform.FindChildRecursive(o.name).gameObject);
                                }
                            }
                        }
                        Destroy(currentLHoldable);
                        currentLHoldable = lHoldable;
                        Lholdable.Value = Path.GetFileName(file);
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomLHoldable", holdableDes.Name);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        lHoldable.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/").transform, false);
                    }
                    Destroy(parentAsset);
                }
                else
                {
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                    if (!lHand)
                    {
                        Destroy(currentRHoldable);
                        currentRHoldable = parentAsset;
                        Rholdable.Value = Path.GetFileName(file);
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomRHoldable", cosmeticName);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
                    }
                    else if (lHand)
                    {
                        Destroy(currentLHoldable);
                        currentLHoldable = parentAsset;
                        Lholdable.Value = Path.GetFileName(file);
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomLHoldable", cosmeticName);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/").transform, false);
                    }
                }
            }
        }

        public void LoadHat(string file)
        {
            if (file == "Disable")
            {
                Destroy(currentHat);
                hat.Value = "";
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomHat", "");
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                return;
            }

            if (!assetCache.TryGetValue(Path.GetFileName(file), out var empty))
            {
                return;
            }

            if (removeCosmetics.Value == true) { RemoveItem(CosmeticsController.CosmeticCategory.Hat, CosmeticsController.CosmeticSlots.Hat); }
            GameObject asset;
            assetCache.TryGetValue(Path.GetFileName(file), out asset);
            GameObject prefab = Instantiate(asset);
            hat.Value = Path.GetFileName(file);
            if (!usingTextMethod)
            {
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomHat", hatDes.Name);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
            else
            {
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomHat", cosmeticName);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
            if (prefab != null)
            {
                var parentAsset = prefab;
                if (!usingTextMethod)
                {
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                    if (hatDes.behaviours.Count > 0)
                    {
                        foreach (CosmeticBehaviour behaviour in hatDes.behaviours)
                        {
                            CustomBehaviour cbehaviour = parentAsset.AddComponent<CustomBehaviour>();
                            cbehaviour.button = behaviour.button;
                            foreach (GameObject o in behaviour.objectsToToggle)
                            {
                                cbehaviour.objectsToToggle.Add(parentAsset.transform.FindChildRecursive(o.name).gameObject);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                }
                if (currentHat != null)
                {
                    Destroy(currentHat);
                }
                currentHat = parentAsset;
                parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/head/").transform, false);
            }
        }
        public void LoadBadge(string file)
        {
            if (file == "Disable")
            {
                Destroy(currentBadge);
                badge.Value = "";
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomBadge", "");
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }

            if (!assetCache.TryGetValue(Path.GetFileName(file), out var empty))
            {
                return;
            }

            if (removeCosmetics.Value == true) { RemoveItem(CosmeticsController.CosmeticCategory.Badge, CosmeticsController.CosmeticSlots.Badge); }
            GameObject asset;
            assetCache.TryGetValue(Path.GetFileName(file), out asset);
            GameObject prefab = Instantiate(asset);
            badge.Value = Path.GetFileName(file);
            if (!usingTextMethod)
            {
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomBadge", badgeDes.Name);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
            else
            {
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomBadge", cosmeticName);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }

            if (prefab != null)
            {
                var parentAsset = prefab;
                foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                {
                    Destroy(collider);
                }
                Destroy(currentBadge);
                currentBadge = parentAsset;
                parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/").transform, false);
            }
        }

        // void OnGUI()
        // {
        //     GUILayout.Label("Custom Properties");
        //     GUILayout.BeginArea(new Rect(10, 10, Screen.width, 500));
        //     if (PhotonNetwork.InRoom)
        //     {
        //         foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        //         {
        //             GUILayout.Label(player.NickName + player.CustomProperties.ToString());
        //         }
        //     }
        //     GUILayout.EndArea();
        // }

        public void CheckPlayer(NetPlayer player, VRRig playerRig)
        {
            try
            {
                if (!playerRig.isLocal)
                {
                    ExitGames.Client.Photon.Hashtable props = PhotonNetwork.CurrentRoom.GetPlayer(player.ID).CustomProperties;
                    Photon.Realtime.Player playerr = PhotonNetwork.CurrentRoom.GetPlayer(player.ID);
                    normalplayers.Add(playerRig, playerr);
                    Debug.Log($"{player.NickName} entered the room");
                    if (props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomLHoldable", out object hold) || props.TryGetValue("CustomRHoldable", out object rhold) || props.TryGetValue("CustomBadge", out object badge) || props.TryGetValue("CustomMaterial", out object material) || props.TryGetValue("CustomTagMaterial", out object tagmaterial))
                    {
                        cosmeticsplayers.Add(playerRig, playerr);
                        SetCosmetics(playerRig, props, playerr);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        public void RemovePlayer(NetPlayer p, VRRig r)
        {
            try
            {
                if (!r.isLocal)
                {
                    Photon.Realtime.Player player = normalplayers[r];
                    ExitGames.Client.Photon.Hashtable props = player.CustomProperties;
                    if(props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomLHoldable", out object hold) || props.TryGetValue("CustomRHoldable", out object rhold) || props.TryGetValue("CustomMaterial", out object material) || props.TryGetValue("CustomTagMaterial", out object tagmaterial))
                    {
                        RemoveCosmetics(props, r, player);
                    }
                    normalplayers.Remove(r);
                }
                else
                {
                    EnableMaterial();
                }
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void RemoveCosmetics(ExitGames.Client.Photon.Hashtable props, VRRig r, Photon.Realtime.Player player)
        {
            if (props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomLHoldable", out object hold) || props.TryGetValue("CustomRHoldable", out object holdr) || props.TryGetValue("CustomBadge", out object badge) || props.TryGetValue("CustomMaterial", out object material) || props.TryGetValue("CustomTagMaterial", out object tagmaterial))
            {
                if(networkHats.ContainsKey(player))
                {
                    Destroy(networkHats[player]);
                    networkHats.Remove(player);
                }
                if (networkLHoldables.ContainsKey(player))
                {
                    Destroy(networkLHoldables[player]);
                    networkLHoldables.Remove(player);
                }
                if (networkRHoldables.ContainsKey(player))
                {
                    Destroy(networkRHoldables[player]);
                    networkRHoldables.Remove(player);
                }
                if (networkBadges.ContainsKey(player))
                {
                    Destroy(networkBadges[player]);
                    networkBadges.Remove(player);
                }
                r.materialsToChangeTo[0] = r.myDefaultSkinMaterialInstance;
                r.materialsToChangeTo[2] = r.myDefaultSkinMaterialInstance;
                Material[] sharedMaterials = r.mainSkin.sharedMaterials;
                sharedMaterials[0] = r.materialsToChangeTo[r.setMatIndex];
                sharedMaterials[1] = r.defaultSkin.chestMaterial;
                r.mainSkin.sharedMaterials = sharedMaterials;
                cosmeticsplayers.Remove(r);
            }
        }

        public void SetCosmetics(VRRig playerRig, ExitGames.Client.Photon.Hashtable props, Photon.Realtime.Player playerr)
        {
            if (playerRig != null)
            {
                if (props.TryGetValue("CustomHat", out object test))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, hat is: {test.ToString()}");
                    if (nameAssetCache.TryGetValue(test.ToString(), out GameObject hat))
                    {
                        LoadNetworkHat(test.ToString(), playerRig, playerr);
                    }
                }
                if (props.TryGetValue("CustomRHoldable", out object r))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, holdable is: {r.ToString()}");
                    if(nameAssetCache.TryGetValue(r.ToString(), out GameObject rholdable))
                    {
                        LoadNetworkHoldable(r.ToString(), playerRig, playerr, false);
                    }
                }
                if (props.TryGetValue("CustomLHoldable", out object l))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, holdable is: {l.ToString()}");
                    if (nameAssetCache.TryGetValue(l.ToString(), out GameObject lholdable))
                    {
                        LoadNetworkHoldable(l.ToString(), playerRig, playerr, true);
                    }
                }
                if (props.TryGetValue("CustomBadge", out object testtt))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, badge is: {testtt.ToString()}");
                    if (nameAssetCache.TryGetValue(testtt.ToString(), out GameObject badge))
                    {
                        LoadNetworkBadge(testtt.ToString(), playerRig, playerr);
                    }
                }
                if (props.TryGetValue("CustomMaterial", out object testttt))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, material is: {testttt.ToString()}");
                    if (nameAssetCache.TryGetValue(testttt.ToString(), out GameObject mat))
                    {
                        LoadNetworkMaterial(testttt.ToString(), 0, playerRig, playerr);
                    }
                }
                if (props.TryGetValue("CustomTagMaterial", out object tagmat))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, tagged material is: {tagmat.ToString()}");
                    if (nameAssetCache.TryGetValue(tagmat.ToString(), out GameObject taggmat))
                    {
                        LoadNetworkMaterial(tagmat.ToString(), 2, playerRig, playerr);
                    }
                }
            }
            else if (playerRig == null)
            {
                Debug.Log("rig is null uh oh");
            }
            else if (playerRig.playerText.gameObject == null)
            {
                Debug.Log("text is null this is not sigma but its fine");
            }
        }

        public void LoadNetworkHat(string file, VRRig rig, Photon.Realtime.Player player)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = Instantiate(asset);
                if (prefab != null)
                {
                    var parentAsset = prefab;
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                    networkHats.Add(player, parentAsset);
                    parentAsset.transform.SetParent(rig.transform.Find("rig/body/head/"), false);
                }
            }
        }
        public void LoadNetworkHoldable(string file, VRRig rig, Photon.Realtime.Player player, bool lHand)
        {
            try
            {
                if (file != "")
                {
                    GameObject asset;
                    nameAssetCache.TryGetValue(file, out asset);
                    GameObject prefab = Instantiate(asset);
                    if (prefab != null)
                    {
                        var parentAsset = prefab;
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                        if(parentAsset.TryGetComponent<Text>(out var text))
                        {
                            if (!lHand)
                            {
                                parentAsset.transform.SetParent(rig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
                                networkRHoldables.Add(player, parentAsset);
                            }
                            else if (lHand)
                            {
                                parentAsset.transform.SetParent(rig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
                                networkLHoldables.Add(player, parentAsset);
                            }
                        }
                        else
                        {
                            if (!lHand)
                            {
                                GameObject rHoldable = Instantiate(parentAsset.GetComponent<HoldableDescriptor>().rightHandObject);
                                rHoldable.transform.SetParent(rig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
                                networkRHoldables.Add(player, rHoldable);
                            }
                            else if (lHand)
                            {
                                GameObject lHoldable = Instantiate(parentAsset.GetComponent<HoldableDescriptor>().leftHandObject);
                                lHoldable.transform.SetParent(rig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
                                networkLHoldables.Add(player, lHoldable);
                            }
                            Destroy(parentAsset);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        public void LoadNetworkBadge(string file, VRRig rig, Photon.Realtime.Player player)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = Instantiate(asset);
                if (prefab != null)
                {
                    var parentAsset = prefab;
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        Destroy(collider);
                    }
                    parentAsset.transform.SetParent(rig.transform.Find("rig/body/"), false);
                    networkBadges.Add(player, parentAsset);
                }
            }
        }
        public void LoadNetworkMaterial(string file, int materialIndex, VRRig rig, Photon.Realtime.Player player)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = Instantiate(asset);
                if (prefab != null)
                {
                    var parentAsset = prefab;
                    try
                    {
                        if (materialIndex == 0)
                        {
                            MaterialDescriptor matInfo = parentAsset.GetComponent<MaterialDescriptor>();
                            if (matInfo.customColors)
                            {
                                parentAsset.GetComponent<MeshRenderer>().material.color = rig.playerColor;
                            }
                            rig.materialsToChangeTo[0] = parentAsset.GetComponent<MeshRenderer>().material;
                            Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                            sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                            sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                            rig.mainSkin.sharedMaterials = sharedMaterials;
                        }
                        else if (materialIndex == 2)
                        {
                            MaterialDescriptor matInfo = parentAsset.GetComponent<MaterialDescriptor>();
                            rig.materialsToChangeTo[materialIndex] = parentAsset.GetComponent<MeshRenderer>().material;
                            Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                            sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                            sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                            rig.mainSkin.sharedMaterials = sharedMaterials;
                        }
                        else
                        {
                            rig.materialsToChangeTo[materialIndex] = currentTaggedMaterial.mat;
                        }
                        Destroy(parentAsset);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            else
            {
                if (materialIndex == 0)
                {
                    rig.materialsToChangeTo[materialIndex] = rig.myDefaultSkinMaterialInstance;
                    Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                    sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                    sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                    rig.mainSkin.sharedMaterials = sharedMaterials;
                }
                else if (materialIndex == 2)
                {
                    rig.materialsToChangeTo[materialIndex] = defaultTaggedMaterial;
                    Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                    sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                    sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                    rig.mainSkin.sharedMaterials = sharedMaterials;
                }
            }
        }

        public async Task<AssetBundle> LoadBundle(string file)
        {
            var bundleLoadRequest = AssetBundle.LoadFromFileAsync(file);

            // AssetBundleCreateRequest is a YieldInstruction !!
            await Yield(bundleLoadRequest);

            AssetBundle _storedBundle = bundleLoadRequest.assetBundle;
            return _storedBundle;
        }

        async Task Yield(YieldInstruction instruction)
        {
            var completionSource = new TaskCompletionSource<YieldInstruction>();
            StartCoroutine(AwaitInstructionCorouutine(instruction, completionSource));
            await completionSource.Task;
        }

        IEnumerator AwaitInstructionCorouutine(YieldInstruction instruction, TaskCompletionSource<YieldInstruction> completionSource)
        {
            yield return instruction;
            completionSource.SetResult(instruction);
        }

        public void CheckItems()
        {
            if(removeCosmetics.Value == true)
            {
                var items = CosmeticsController.instance.currentWornSet.items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].itemCategory == CosmeticsController.CosmeticCategory.Hat)
                    {
                        LoadHat("Disable");
                    }
                    if (items[i].itemCategory == CosmeticsController.CosmeticCategory.Badge)
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
                bool updateCart = false;

                var nullItem = CosmeticsController.instance.nullItem;

                var items = CosmeticsController.instance.currentWornSet.items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].itemCategory == category && !items[i].isNullItem)
                    {
                        updateCart = true;
                        items[i] = nullItem;
                    }
                }

                items = CosmeticsController.instance.tryOnSet.items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].itemCategory == category && !items[i].isNullItem)
                    {
                        updateCart = true;
                        items[i] = nullItem;
                    }
                }

                // TODO: Check if this call is necessary
                if (updateCart)
                {
                    CosmeticsController.instance.UpdateShoppingCart();
                    CosmeticsController.instance.UpdateWornCosmetics(true);

                    PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), nullItem.itemName);
                    PlayerPrefs.Save();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to remove game cosmetic\n{e.GetType().Name} ({e.Message})");
            }
        }

        class Net : MonoBehaviourPunCallbacks
        {
            public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
            {
                base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

                if (targetPlayer.IsLocal) return;

                NetPlayer player = NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber);
                Plugin.instance.RemoveCosmetics(changedProps, GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), targetPlayer);
                Plugin.instance.SetCosmetics(GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), changedProps, targetPlayer);
            }
        }

        public void LoadMaterial(string file, int materialIndex)
        {
            if (file == "Disable")
            {
                if(materialIndex == 0)
                {
                    material.Value = "";
                    currentMaterial.mat = null;
                    var table = PhotonNetwork.LocalPlayer.CustomProperties;
                    table.AddOrUpdate("CustomMaterial", "");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                    VRRig rig = GorillaTagger.Instance.offlineVRRig;
                    rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
                    Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                    sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                    sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                    rig.mainSkin.sharedMaterials = sharedMaterials;
                }
                else if(materialIndex == 2)
                {
                    taggedMaterial.Value = "";
                    currentTaggedMaterial.mat = null;
                    var table = PhotonNetwork.LocalPlayer.CustomProperties;
                    table.AddOrUpdate("CustomTagMaterial", "");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                    VRRig rig = GorillaTagger.Instance.offlineVRRig;
                    rig.materialsToChangeTo[2] = defaultTaggedMaterial;
                    Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                    sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                    sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                    rig.mainSkin.sharedMaterials = sharedMaterials;
                }
            }

            if (!assetCache.TryGetValue(Path.GetFileName(file), out var empty))
            {
                return;
            }

            if (materialIndex == 0)
            {
                material.Value = Path.GetFileName(file);
            }
            else if (materialIndex == 2)
            {
                taggedMaterial.Value = Path.GetFileName(file);
            }
            GameObject asset;
            assetCache.TryGetValue(Path.GetFileName(file), out asset);
            GameObject prefab = Instantiate(asset);
            RemoveItem(CosmeticsController.CosmeticCategory.Fur, CosmeticsController.CosmeticSlots.Fur);
            if (prefab != null)
            {
                var parentAsset = prefab;
                try
                {
                    if (materialIndex == 0)
                    {
                        VRRig rig = GorillaTagger.Instance.offlineVRRig;
                        currentMaterial.mat = parentAsset.GetComponent<MeshRenderer>().material;
                        if (usingTextMethod)
                        {
                            currentMaterial.customColours = materialCustomColours;
                        }
                        else
                        {
                            currentMaterial.customColours = matDes.customColors;
                        }

                        if (currentMaterial.customColours)
                        {
                            currentMaterial.mat.color = rig.playerColor;
                        }
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomMaterial", matDes.Name);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        rig.materialsToChangeTo[materialIndex] = currentMaterial.mat;
                        Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                        sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                        sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                        rig.mainSkin.sharedMaterials = sharedMaterials;
                    }
                    else if (materialIndex == 2)
                    {
                        currentTaggedMaterial.mat = parentAsset.GetComponent<MeshRenderer>().material;
                        if (usingTextMethod)
                        {
                            currentMaterial.customColours = materialCustomColours;
                        }
                        else
                        {
                            currentMaterial.customColours = matDes.customColors;
                        }
                        VRRig rig = GorillaTagger.Instance.offlineVRRig;
                        if (currentTaggedMaterial.customColours)
                        {
                            currentTaggedMaterial.mat.color = new Color(1f, 0.4f, 0f);
                        }
                        var table = PhotonNetwork.LocalPlayer.CustomProperties;
                        table.AddOrUpdate("CustomTagMaterial", matDes.Name);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                        rig.materialsToChangeTo[materialIndex] = currentTaggedMaterial.mat;
                        Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                        sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                        sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                        rig.mainSkin.sharedMaterials = sharedMaterials;
                    }
                    else
                    {
                        VRRig rig = GorillaTagger.Instance.offlineVRRig;
                        rig.materialsToChangeTo[materialIndex] = currentTaggedMaterial.mat;
                    }
                    Destroy(parentAsset);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        public string GetCosName(GameObject obj, string mode)
        {
            string cosname = "Empty";

            if (obj.TryGetComponent(out Text values))
            {
                usingTextMethod = true;
                string[] info = values.text.Split("$");
                switch (mode)
                {
                    case "Material":
                        cosname = info[0];
                        break;
                    case "Holdable":
                        cosname = info[0];
                        break;
                    case "Badge":
                        cosname = info[0];
                        break;
                    case "Hat":
                        cosname = info[0];
                        break;
                }
            }
            else
            {
                usingTextMethod = false;
                switch (mode)
                {
                    case "Material":
                        cosname = obj.GetComponent<MaterialDescriptor>().Name;
                        break;
                    case "Holdable":
                        cosname = obj.GetComponent<HoldableDescriptor>().Name;
                        break;
                    case "Hat":
                        cosname = obj.GetComponent<HatDescriptor>().Name;
                        break;
                    case "Badge":
                        cosname = obj.GetComponent<BadgeDescriptor>().Name;
                        break;
                }
            }
            return cosname;
        }

        public void EnableMaterial()
        {
            VRRig rig = GorillaTagger.Instance.offlineVRRig;
            currentMaterial.mat.color = rig.playerColor;
            rig.materialsToChangeTo[0] = currentMaterial.mat;
            Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
            sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
            sharedMaterials[1] = rig.defaultSkin.chestMaterial;
            rig.mainSkin.sharedMaterials = sharedMaterials;
            if (currentTaggedMaterial.customColours && currentTaggedMaterial.mat != null)
            {
                currentTaggedMaterial.mat.color = new Color(1f, 0.4f, 0f);
            }
        }

        public void EnableNetworkMaterial(VRRig rig)
        {
            Photon.Realtime.Player p;
            if(cosmeticsplayers.TryGetValue(rig, out p))
            {
                if (p.CustomProperties.TryGetValue("CustomMaterial", out object mat))
                {
                    LoadNetworkMaterial(mat.ToString(), 0, rig, p);
                }
                if (p.CustomProperties.TryGetValue("CustomTagMaterial", out object tagmat))
                {
                    LoadNetworkMaterial(tagmat.ToString(), 2, rig, p);
                }
            }
        }

        public void UpdateColour(Color colour)
        {
            if (currentMaterial.mat != null)
            {
                VRRig rig = GorillaTagger.Instance.offlineVRRig;
                rig.materialsToChangeTo[0] = currentMaterial.mat;
                if (currentMaterial.customColours)
                {
                    currentMaterial.mat.color = colour;
                }
                Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
                sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
                sharedMaterials[1] = rig.defaultSkin.chestMaterial;
                rig.mainSkin.sharedMaterials = sharedMaterials;
            }
        }

        public void GetInfo(string file, string mode)
        {
            GameObject cosmetic;
            string[] info;
            assetCache.TryGetValue(file, out cosmetic);

            if (cosmetic.TryGetComponent(out Text values))
            {
                usingTextMethod = true;
                info = values.text.Split("$");
                switch (mode)
                {
                    case "Material":
                        currentCosmeticFile = file;
                        cosmeticName = info[0];
                        cosmeticAuthor = info[1];
                        cosmeticDescription = info[2];
                        materialCustomColours = info[3].ToUpper() == "TRUE";
                        break;
                    case "Holdable":
                        currentCosmeticFile = file;
                        cosmeticName = info[0];
                        cosmeticAuthor = info[1];
                        cosmeticDescription = info[2];
                        leftHand = info[3].ToUpper() == "TRUE";
                        break;
                    case "Badge":
                        currentCosmeticFile = file;
                        cosmeticName = info[0];
                        cosmeticAuthor = info[1];
                        cosmeticDescription = info[2];
                        break;
                    case "Hat":
                        currentCosmeticFile = file;
                        cosmeticName = info[0];
                        cosmeticAuthor = info[1];
                        cosmeticDescription = info[2];
                        break;
                }
            }
            else
            {
                usingTextMethod = false;
                switch (mode)
                {
                    case "Material":
                        currentCosmeticFile = file;
                        matDes = cosmetic.GetComponent<MaterialDescriptor>();
                        break;
                    case "Holdable":
                        currentCosmeticFile = file;
                        holdableDes = cosmetic.GetComponent<HoldableDescriptor>();
                        break;
                    case "Hat":
                        currentCosmeticFile = file;
                        hatDes = cosmetic.GetComponent<HatDescriptor>();
                        break;
                    case "Badge":
                        currentCosmeticFile = file;
                        badgeDes = cosmetic.GetComponent<BadgeDescriptor>();
                        break;
                }
            }
        }
    }
}