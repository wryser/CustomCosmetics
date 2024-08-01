using System.Collections.Generic;
using UnityEngine;

public class CosmeticBehaviour : MonoBehaviour
{
    public string button;
    public bool useTrigger = false;
    public Collider trigger;
    public List<GameObject> objectsToToggle = new List<GameObject>();

}