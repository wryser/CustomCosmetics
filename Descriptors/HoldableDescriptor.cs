using CustomCosmetics;
using System.Collections.Generic;
using UnityEngine;

public class HoldableDescriptor : MonoBehaviour
{
    public string Name = "";
    public string Author = "";
    public string Description = "";
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public bool customColours = false;
    public string id;

    public List<CosmeticBehaviour> behaviours = new List<CosmeticBehaviour>();

    public bool gunEnabled = false;
    public bool audioMode;
    public AudioClip shootSound;
    public float bulletSpeed;
    public float bulletCooldown;
    public GameObject bulletObject;

    public bool vibra;
    public float strenth = 0.25f;
    public float sTime = 0.5f;

    public float bulletMultiply;
}
