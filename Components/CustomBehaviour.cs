using CustomCosmetics.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomCosmetics
{
    public class CustomBehaviour : MonoBehaviour
    {
        public bool usingTrigger;
        public bool pressingButton;
        public bool justPressed;
        public string button;
        public Collider trigger;
        public List<GameObject> objectsToToggle = new List<GameObject>();

        public void Start()
        {
            if(usingTrigger)
            {
                trigger.AddComponent<Trigger>().behaviour = this;
            }
        }

        void Update()
        {
            switch (button)
            {
                case "LeftTrigger":
                    pressingButton = ControllerInputPoller.instance.leftControllerIndexFloat > 0.3f;
                    break;
                case "RightTrigger":
                    pressingButton = ControllerInputPoller.instance.rightControllerIndexFloat > 0.3f;
                    break;
                case "LeftGrip":
                    pressingButton = ControllerInputPoller.instance.leftGrab;
                    break;
                case "RightGrip":
                    pressingButton = ControllerInputPoller.instance.rightGrab;
                    break;
                case "Y":
                    pressingButton = ControllerInputPoller.instance.leftControllerSecondaryButton;
                    break;
                case "X":
                    pressingButton = ControllerInputPoller.instance.leftControllerPrimaryButton;
                    break;
                case "B":
                    pressingButton = ControllerInputPoller.instance.rightControllerPrimaryButton;
                    break;
                case "A":
                    pressingButton = ControllerInputPoller.instance.rightControllerPrimaryButton;
                    break;
            }

            if(!usingTrigger)
            {
                if (pressingButton && !justPressed)
                {
                    for (int i = 0; i < objectsToToggle.Count; i++)
                    {
                        objectsToToggle[i].SetActive(!objectsToToggle[i].activeSelf);
                    }
                    justPressed = true;
                }
                else if (!pressingButton && justPressed)
                {
                    justPressed = false;
                }
            }
        }
    }
}
