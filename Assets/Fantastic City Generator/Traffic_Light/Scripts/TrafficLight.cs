using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FCG
{
    public class TrafficLight : MonoBehaviour
    {
        public GameObject Green;
        public GameObject Yellow;
        public GameObject Red;
        public GameObject Pedestrians;
        public GameObject StopCollider;
        public GameObject StopPedestrianCollider;

        public void SetStatus(string status)
        {

            Red.SetActive(status == "1");
            Yellow.SetActive(status == "2");
            Green.SetActive(status == "3");
            Pedestrians.SetActive(status == "4");
            StopCollider.SetActive(status != "3");
            StopPedestrianCollider.SetActive(status != "4");

        }
    }
}