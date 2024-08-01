using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomCosmetics.Components
{
    public class Trigger : MonoBehaviour
    {
        public CustomBehaviour behaviour;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 10 && behaviour.usingTrigger)
            {
                if (behaviour.pressingButton && !behaviour.justPressed)
                {
                    for (int i = 0; i < behaviour.objectsToToggle.Count; i++)
                    {
                        behaviour.objectsToToggle[i].SetActive(!behaviour.objectsToToggle[i].activeSelf);
                    }
                    behaviour.justPressed = true;
                }
                else if (!behaviour.pressingButton && behaviour.justPressed)
                {
                    behaviour.justPressed = false;
                }
            }
        }
    }
}
