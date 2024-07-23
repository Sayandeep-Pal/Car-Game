
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FCG
{
    public class TrafficLights2 : MonoBehaviour
    {
        private float countTime = 0;
        private int step = 0;

        private int status;

        public TrafficLight trafficLight_N;
        public TrafficLight trafficLight_S;
        public TrafficLight trafficLight_E;
        public TrafficLight trafficLight_W;

        // Use this for initialization
        void Start()
        {

            countTime = 0;
            step = 0;

            status = (Random.Range(1, 8) < 4) ? 13 : 31;

            EnabledObjects(status);

            InvokeRepeating(nameof(TrafficLightTurn), Random.Range(0, 4), 1);


        }



        private void TrafficLightTurn()
        {
            countTime += 1;

            if (step == 0)
            {

                if (countTime > 16) // How many seconds will the signal turn red or green
                {
                    countTime = 0;
                    step = 1;

                    if (status == 13)
                        status = 12;
                    else if (status == 31)
                        status = 21;

                    EnabledObjects(status);

                }

            }
            else if (step == 1)
            {

                if (countTime >= 5)  //How many seconds will the signal turn yellow 
                {
                    countTime = 0;
                    step = 2;

                    if (status == 12)
                        status = 41;
                    else if (status == 21)
                        status = 14;
                    EnabledObjects(44);

                }

            }
            else if (step == 2)
            {

                if (countTime >= 7) // How many seconds will it be open for pedestrians to cross the street?
                {
                    countTime = 0;
                    step = 0;

                    if (status == 14)
                        status = 13;
                    else if (status == 41)
                        status = 31;

                    EnabledObjects(status);
                }

            }


        }


        void EnabledObjects(int st)
        {

            if (trafficLight_N)
                trafficLight_N.SetStatus(st.ToString().Substring(0, 1));

            if (trafficLight_S)
                trafficLight_S.SetStatus(st.ToString().Substring(0, 1));

            if (trafficLight_E)
                trafficLight_E.SetStatus(st.ToString().Substring(1, 1));

            if (trafficLight_W)
                trafficLight_W.SetStatus(st.ToString().Substring(1, 1));

        }

    }
}