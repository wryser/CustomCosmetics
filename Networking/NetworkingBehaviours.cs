using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static CustomCosmetics.Extensions.ColorExtensions;

namespace CustomCosmetics.Networking
{
    class NetworkingBehaviours
    {
        // Helps to access players using custom cosmetics
        public static Dictionary<VRRig, Photon.Realtime.Player> cosmeticsplayers = new Dictionary<VRRig, Photon.Realtime.Player>();

        // Holds all cosmetics (accessible by file name)
        public static Dictionary<string, GameObject> assetCache = new Dictionary<string, GameObject>();
        // Holds all cosmetics (accessible by name this one is mainly used for networking)
        public static Dictionary<string, GameObject> nameAssetCache = new Dictionary<string, GameObject>();

        // Struct for holding the custom material info and the current custom material/materials
        public struct customMaterial
        {
            public Material mat;
            public bool customColours;
        }

        // Current Equipped Cosmetics
        public static GameObject currentRHoldable;
        public static GameObject currentLHoldable;
        public static GameObject currentHat;
        public static GameObject currentBadge;
        public static customMaterial currentMaterial = new customMaterial();
        public static customMaterial currentTaggedMaterial = new customMaterial();
        public static Material defaultTaggedMaterial;

        // Dictionarys to help access players networked cosmetics
        public static Dictionary<Photon.Realtime.Player, GameObject> networkHats = new Dictionary<Photon.Realtime.Player, GameObject>();
        public static Dictionary<Photon.Realtime.Player, GameObject> networkRHoldables = new Dictionary<Photon.Realtime.Player, GameObject>();
        public static Dictionary<Photon.Realtime.Player, GameObject> networkLHoldables = new Dictionary<Photon.Realtime.Player, GameObject>();
        public static Dictionary<Photon.Realtime.Player, GameObject> networkBadges = new Dictionary<Photon.Realtime.Player, GameObject>();

        public static void EnableNetworkMaterial(VRRig rig)
        {
            Photon.Realtime.Player p;
            if (cosmeticsplayers.TryGetValue(rig, out p))
            {
                if (p.CustomProperties.TryGetValue("CustomMaterial", out object mat))
                {
                    p.CustomProperties.TryGetValue("Colour", out object co);
                    Color col = parseColor(co.ToString());
                    LoadNetworkMaterial(mat.ToString(), 0, rig, p, col);
                }
                if (p.CustomProperties.TryGetValue("CustomTagMaterial", out object tagmat))
                {
                    p.CustomProperties.TryGetValue("Colour", out object color);
                    Color c = parseColor(color.ToString());
                    LoadNetworkMaterial(tagmat.ToString(), 2, rig, p, c);
                }
            }
        }

        public static void LoadNetworkHat(string file, VRRig rig, Photon.Realtime.Player player)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = UnityEngine.Object.Instantiate(asset);
                if (prefab != null)
                {
                    var parentAsset = prefab;
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        UnityEngine.Object.Destroy(collider);
                    }
                    networkHats.Add(player, parentAsset);
                    parentAsset.transform.SetParent(rig.transform.Find("rig/body/head/"), false);
                }
            }
        }
        public static void LoadNetworkHoldable(string file, VRRig rig, Photon.Realtime.Player player, bool lHand)
        {
            try
            {
                if (file != "")
                {
                    GameObject asset;
                    nameAssetCache.TryGetValue(file, out asset);
                    GameObject prefab = UnityEngine.Object.Instantiate(asset);
                    if (prefab != null)
                    {
                        var parentAsset = prefab;
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            UnityEngine.Object.Destroy(collider);
                        }
                        if (parentAsset.TryGetComponent<Text>(out var text))
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
                                GameObject rHoldable = UnityEngine.Object.Instantiate(parentAsset.GetComponent<HoldableDescriptor>().rightHandObject);
                                rHoldable.transform.SetParent(rig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/"), false);
                                networkRHoldables.Add(player, rHoldable);
                            }
                            else if (lHand)
                            {
                                GameObject lHoldable = UnityEngine.Object.Instantiate(parentAsset.GetComponent<HoldableDescriptor>().leftHandObject);
                                lHoldable.transform.SetParent(rig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/"), false);
                                networkLHoldables.Add(player, lHoldable);
                            }
                            UnityEngine.Object.Destroy(parentAsset);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        public static void LoadNetworkBadge(string file, VRRig rig, Photon.Realtime.Player player)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = UnityEngine.Object.Instantiate(asset);
                if (prefab != null)
                {
                    var parentAsset = prefab;
                    foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                    {
                        UnityEngine.Object.Destroy(collider);
                    }
                    parentAsset.transform.SetParent(rig.transform.Find("rig/body/"), false);
                    networkBadges.Add(player, parentAsset);
                }
            }
        }
        public static void LoadNetworkMaterial(string file, int materialIndex, VRRig rig, Photon.Realtime.Player player, Color colour)
        {
            if (file != "")
            {
                GameObject asset;
                nameAssetCache.TryGetValue(file, out asset);
                GameObject prefab = UnityEngine.Object.Instantiate(asset);
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
                        UnityEngine.Object.Destroy(parentAsset);
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
    }
}
