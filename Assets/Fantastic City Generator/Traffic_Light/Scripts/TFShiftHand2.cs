using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FCG
{
    public class TFShiftHand2 : MonoBehaviour
    {
        public TrafficLights2 rightHandObjects;
        public TrafficLights2 leftHandObjects;
        public TrafficLights2 leftHandObjectsJapan;

        public void RightHand(int active)
        {

            rightHandObjects.gameObject.SetActive(active == 0);
            leftHandObjects.gameObject.SetActive(active == 1);

            if (leftHandObjectsJapan)
                leftHandObjectsJapan.gameObject.SetActive(active == 2);

        }

    }
}