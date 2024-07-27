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
        // Material currentMaterial;
        public string cosmeticPath = Application.dataPath + "/../BepInEx/Cosmetics";
        public static ConfigEntry<string> hat;
        public static ConfigEntry<string> holdable;
        public static ConfigEntry<string> badge;
        // public static ConfigEntry<string> material;
        public static ConfigEntry<bool> multipleHats;
        List<GameObject> MultipleHats = new List<GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkHats = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkHoldables = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<Photon.Realtime.Player, GameObject> networkBadges = new Dictionary<Photon.Realtime.Player, GameObject>();
        Dictionary<VRRig, Photon.Realtime.Player> cosmeticsplayers = new Dictionary<VRRig, Photon.Realtime.Player>();
        Dictionary<VRRig, Photon.Realtime.Player> normalplayers = new Dictionary<VRRig, Photon.Realtime.Player>();

        void Awake()
        {
            SceneManager.sceneLoaded += GameInitialized;
            instance = this;
        }

        public bool GetMultipleHats()
        {
            bool multipleHatsBool;
            multipleHatsBool = multipleHats.Value;
            return multipleHatsBool;
        }
        public bool ToggleMultipleHats()
        {
            bool multipleHatsBool;
            multipleHats.Value = !multipleHats.Value;
            multipleHatsBool = multipleHats.Value;
            return multipleHatsBool;
        }

        void GameInitialized(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "GorillaTag")
            {
                /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
                multipleHats = Config.Bind("Settings", "Multiple Hats", false, "This allows you to use multiple hats at once");
                hat = Config.Bind("Cosmetics", "Current Hat", "", "This is the current hat your using.");
                holdable = Config.Bind("Cosmetics", "Current Holdable", "", "This is the current holdable your using.");
                badge = Config.Bind("Cosmetics", "Current Badge", "", "This is the current badge your using.");
                // material = Config.Bind("Cosmetics", "Current Material", "", "This is the current material your using.");
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
                // if (!Directory.Exists(cosmeticPath + "/Materials"))
                // {
                //     Directory.CreateDirectory(cosmeticPath + "/Materials");
                // }
                string savedhat = hat.Value;
                string savedholdable = holdable.Value;
                string savedbadge = badge.Value;
                // string savedmaterial = material.Value;
                if (savedhat != "")
                {
                    LoadHat(cosmeticPath + "/Hats/" + savedhat);
                }
                if (savedholdable != "")
                {
                    LoadHoldable(cosmeticPath + "/Holdables/" + savedholdable);
                }
                if (savedbadge != "")
                {
                    LoadBadge(cosmeticPath + "/Badges/" + savedbadge);
                }
                // if (savedmaterial != "")
                // {
                //     LoadMaterial(cosmeticPath + "/Materials/" + savedmaterial);
                // }
                this.AddComponent<Net>();
                Harmony harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.GUID);
                Type rigCache = typeof(GorillaTagger).Assembly.GetType("VRRigCache");
                harmony.Patch(AccessTools.Method(rigCache, "AddRigToGorillaParent"), postfix: new HarmonyMethod(typeof(RigCreatePatch), nameof(RigCreatePatch.Patch)));
                harmony.Patch(AccessTools.Method(rigCache, "RemoveRigFromGorillaParent"), postfix: new HarmonyMethod(typeof(RigRemovePatch), nameof(RigRemovePatch.Patch)));
            }
        }

        public void LoadHoldable(string file)
        {
            if (file == "DisableR")
            {
                Destroy(currentRHoldable);
                holdable.Value = "";
            }
            else if (file == "DisableL")
            {
                Destroy(currentLHoldable);
                holdable.Value = "";
            }
            else
            {
                AssetBundle homebundle = AssetBundle.LoadFromFile(file);
                holdable.Value = Path.GetFileName(file);
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomHoldable", Path.GetFileName(file));
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                if (homebundle != null)
                {
                    var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("holdABLE");
                    GameObject prefab = assetLoadRequest.asset as GameObject;

                    GameObject assethome = prefab;
                    if (assethome != null)
                    {
                        var parentAsset = Instantiate(assethome);
                        foreach(Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }

                        homebundle.Unload(false);
                        string[] holdableInfo = parentAsset.GetComponent<Text>().text.Split('$');
                        if (holdableInfo[3].ToUpper() == "FALSE")
                        {
                            Destroy(currentRHoldable);
                            currentRHoldable = parentAsset;
                            parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
                        }
                        else if (holdableInfo[3].ToUpper() == "TRUE")
                        {
                            Destroy(currentLHoldable);
                            currentLHoldable = parentAsset;
                            parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/").transform, false);
                        }
                    }
                }
            }
        }

        public void LoadHat(string file)
        {
            if (file == "Disable")
            {
                Destroy(currentHat);
                for (int i = 0; MultipleHats.Count > i; i++)
                {
                    Destroy(MultipleHats[i]);
                }
                hat.Value = "";
            }
            else
            {
                AssetBundle homebundle = AssetBundle.LoadFromFile(file);
                hat.Value = Path.GetFileName(file);
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomHat", Path.GetFileName(file));
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                if (homebundle != null)
                {
                    var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("hat");
                    GameObject prefab = assetLoadRequest.asset as GameObject;

                    GameObject assethome = prefab;
                    if (assethome != null)
                    {
                        var parentAsset = Instantiate(assethome);
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                        homebundle.Unload(false);
                        if (multipleHats.Value == false)
                        {
                            Destroy(currentHat);
                            for (int i = 0; MultipleHats.Count > i; i++)
                            {
                                Destroy(MultipleHats[i]);
                            }
                            currentHat = parentAsset;
                        }
                        else
                        {
                            MultipleHats.Add(parentAsset);
                        }
                        parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/head/").transform, false);
                    }
                }
            }
        }
        public void LoadBadge(string file)
        {
            if (file == "Disable")
            {
                Destroy(currentBadge);
                badge.Value = "";
            }
            else
            {
                AssetBundle homebundle = AssetBundle.LoadFromFile(file);
                badge.Value = Path.GetFileName(file);
                var table = PhotonNetwork.LocalPlayer.CustomProperties;
                table.AddOrUpdate("CustomBadge", Path.GetFileName(file));
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                if (homebundle != null)
                {
                    var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("badge");
                    GameObject prefab = assetLoadRequest.asset as GameObject;

                    GameObject assethome = prefab;
                    if (assethome != null)
                    {
                        var parentAsset = Instantiate(assethome);
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                        homebundle.Unload(false);
                        Destroy(currentBadge);
                        currentBadge = parentAsset;
                        parentAsset.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/").transform, false);
                    }
                }
            }
        }
        private void OnGUI()
        {
            GUILayout.Label("Custom Properties");
            GUILayout.BeginArea(new Rect(10, 10, Screen.width, 500));
            if(PhotonNetwork.InRoom)
            {
                foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GUILayout.Label(player.NickName + player.CustomProperties.ToString());
                }
            }
            GUILayout.EndArea();
        }

        public void RegisterPlayer(NetPlayer player, VRRig playerRig)
        {
            if (!playerRig.isLocal)
            {
                Hashtable props = PhotonNetwork.CurrentRoom.GetPlayer(player.ID).CustomProperties;
                Photon.Realtime.Player playerr = PhotonNetwork.CurrentRoom.GetPlayer(player.ID);
                normalplayers.Add(playerRig, playerr);
                Debug.Log($"{player.NickName} entered the room");
                if(props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomHoldable", out object hold) || props.TryGetValue("CustomBadge", out object badge))
                {
                    cosmeticsplayers.Add(playerRig, playerr);
                    SetCosmetics(playerRig, props, playerr);
                }
            }
        }
        public void UnregisterPlayer(NetPlayer p, VRRig r)
        {
            Photon.Realtime.Player player = normalplayers[r];
            try
            {
                if (!r.isLocal)
                {
                    Hashtable props = player.CustomProperties;
                    if(props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomHoldable", out object hold) || props.TryGetValue("CustomBadge", out object badge))
                    {
                        RemoveCosmetics(props, r, player);
                    }
                    normalplayers.Remove(r);
                }
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void RemoveCosmetics(Hashtable props, VRRig r, Photon.Realtime.Player player)
        {
            if (props.TryGetValue("CustomHat", out object hat) || props.TryGetValue("CustomHoldable", out object hold) || props.TryGetValue("CustomBadge", out object badge))
            {
                if(networkHats[player] != null)
                {
                    Destroy(networkHats[player]);
                    networkHats.Remove(player);
                }
                if (networkHoldables[player] != null)
                {
                    Destroy(networkHoldables[player]);
                    networkHoldables.Remove(player);
                }
                if (networkBadges[player] != null)
                {
                    Destroy(networkBadges[player]);
                    networkBadges.Remove(player);
                }
                cosmeticsplayers.Remove(r);
            }
        }

        public void SetCosmetics(VRRig playerRig, Hashtable props, Photon.Realtime.Player playerr)
        {
            if (playerRig != null)
            {
                if (props.TryGetValue("CustomHat", out object test))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, hat is: {test.ToString()}");
                    if (File.Exists($"{cosmeticPath}/Hats/{test}"))
                    {
                        LoadNetworkHat($"{cosmeticPath}/Hats/{test.ToString()}", playerRig, playerr);
                    }
                }
                if (props.TryGetValue("CustomHoldable", out object testt))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, holdable is: {testt.ToString()}");
                    if (File.Exists($"{cosmeticPath}/Holdables/{testt}"))
                    {
                        LoadNetworkHoldable($"{cosmeticPath}/Holdables/{testt.ToString()}", playerRig, playerr);
                    }
                }
                if (props.TryGetValue("CustomBadge", out object testtt))
                {
                    Debug.Log($"{playerr.NickName} is using Custom Cosmetics, badge is: {testtt.ToString()}");
                    if (File.Exists($"{cosmeticPath}/Badges/{testtt}"))
                    {
                        LoadNetworkBadge($"{cosmeticPath}/Badges/{testtt.ToString()}", playerRig, playerr);
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
                AssetBundle homebundle = AssetBundle.LoadFromFile(file);

                if (homebundle != null)
                {
                    var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("hat");
                    GameObject prefab = assetLoadRequest.asset as GameObject;

                    GameObject assethome = prefab;
                    if (assethome != null)
                    {
                        var parentAsset = Instantiate(assethome);
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                        homebundle.Unload(false);
                        networkHats.Add(player, parentAsset);
                        parentAsset.transform.SetParent(rig.transform.Find("rig/body/head/"), false);
                    }
                }
            }
        }
        public void LoadNetworkHoldable(string file, VRRig rig, Photon.Realtime.Player player)
        {
            try
            {
                if (file != "")
                {
                    AssetBundle homebundle = AssetBundle.LoadFromFile(file);
                    if (homebundle != null)
                    {
                        var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("holdABLE");
                        GameObject prefab = assetLoadRequest.asset as GameObject;

                        GameObject assethome = prefab;
                        if (assethome != null)
                        {
                            var parentAsset = Instantiate(assethome);
                            foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                            {
                                Destroy(collider);
                            }
                            homebundle.Unload(false);
                            string[] holdableInfo = parentAsset.GetComponent<Text>().text.Split('$');
                            if (holdableInfo[3].ToUpper() == "FALSE")
                            {
                                parentAsset.transform.SetParent(rig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
                            }
                            else if (holdableInfo[3].ToUpper() == "TRUE")
                            {
                                parentAsset.transform.SetParent(rig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
                            }
                            networkHoldables.Add(player, parentAsset);
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
                AssetBundle homebundle = AssetBundle.LoadFromFile(file);
                if (homebundle != null)
                {
                    var assetLoadRequest = homebundle.LoadAssetAsync<GameObject>("badge");
                    GameObject prefab = assetLoadRequest.asset as GameObject;

                    GameObject assethome = prefab;
                    if (assethome != null)
                    {
                        var parentAsset = Instantiate(assethome);
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                        homebundle.Unload(false);
                        parentAsset.transform.SetParent(rig.transform.Find("rig/body/"), false);
                        networkBadges.Add(player, parentAsset);
                    }
                }
            }
        }

        class Net : MonoBehaviourPunCallbacks
        {
            public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
            {
                base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

                if (targetPlayer.IsLocal) return;

                NetPlayer player = NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber);
                Plugin.instance.RemoveCosmetics(changedProps, GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), targetPlayer);
                Plugin.instance.SetCosmetics(GorillaGameManager.instance.FindPlayerVRRig(targetPlayer), changedProps, targetPlayer);
            }
        }
        // public void LoadMaterial(string file)
        // {
        //     if (file == "Disable")
        //     {
        //         material.Value = "";
        //     }
        //     else
        //     {
        //         AssetBundle homebundle = AssetBundle.LoadFromFile(file);
        //         material.Value = Path.GetFileName(file);
        //         if (homebundle != null)
        //         {
        //             GameObject assethome = homebundle.LoadAsset<GameObject>("material");
        //             if (assethome != null)
        //             {
        //                 var parentAsset = Instantiate(assethome);
        //                 homebundle.Unload(false);
        //                 currentMaterial = parentAsset.GetComponent<MeshRenderer>().material;
        //                 GorillaTagger.Instance.offlineVRRig.mainSkin.material = currentMaterial;
        //             }
        //         }
        //     }
        // }
    }
}
