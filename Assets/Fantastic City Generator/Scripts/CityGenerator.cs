using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.IO;
using System.Linq;
using UnityEditor;

namespace FCG
{
    public class CityGenerator : MonoBehaviour
    {

        private int nB = 0;
        private Vector3 center;
        private int residential = 0;
        private bool _residential = false;

        GameObject cityMaker;

        [HideInInspector]
        public GameObject[] miniBorder;

        [HideInInspector]
        public GameObject[] smallBorder;

        [HideInInspector]
        public GameObject[] mediumBorder;

        [HideInInspector]
        public GameObject[] largeBorder;
        
        [HideInInspector]
        public GameObject[] miniBorderFlat;

        [HideInInspector]
        public GameObject[] smallBorderFlat;

        [HideInInspector]
        public GameObject[] mediumBorderFlat;

        [HideInInspector]
        public GameObject[] largeBorderFlat;
         
        [HideInInspector]
        public GameObject[] miniBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] smallBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] mediumBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] largeBorderWithExitOfCity;

        [HideInInspector]
        public GameObject[] largeBlocks;

        private bool[] _largeBlocks;



        [HideInInspector]
        public GameObject[] bigLargeBlocks;


        [HideInInspector]
        public GameObject[] forward50;
        [HideInInspector]
        public GameObject[] forward100;
        [HideInInspector]
        public GameObject[] forward300;
        [HideInInspector]
        public GameObject[] forward400;
        [HideInInspector]
        public GameObject[] forwardLeft400;
        [HideInInspector]
        public GameObject[] forwardRight400;
        [HideInInspector]
        public GameObject[] left200;
        [HideInInspector]
        public GameObject[] left300;
        [HideInInspector]
        public GameObject[] right200;
        [HideInInspector]
        public GameObject[] right300;



        private bool[] _bigLargeBlocks;


        [HideInInspector]
        public GameObject[] BB;  // Buildings in suburban areas (not in the corner)
        [HideInInspector]
        public GameObject[] BC;  // Down Town Buildings(Not in the corner)
        [HideInInspector]
        public GameObject[] BR;  // Residential buildings in suburban areas (not in the corner)
        [HideInInspector]
        public GameObject[] DC;  // Corner buildings that occupy both sides of the block
        [HideInInspector]
        public GameObject[] EB;  // Corner buildings in suburban areas
        [HideInInspector]
        public GameObject[] EC;  // Down Town Corner Buildings 
        [HideInInspector]
        public GameObject[] MB;  //  Buildings that occupy both sides of the block 
        [HideInInspector]
        public GameObject[] BK;  //  Buildings that occupy an entire block
        [HideInInspector]
        public GameObject[] SB;  //  Large buildings that occupy larger blocks 
        [HideInInspector]
        public GameObject[] BBS;  //  Buildings on slopes (neighborhood)
        [HideInInspector]
        public GameObject[] BCS;  //  Down Town Buildings on slopes

        private int[] _BB;
        private int[] _BC;
        private int[] _BR;
        //private int[] _DC;
        private int[] _EB;
        private int[] _EC;

        private int[] _EBS;
        private int[] _ECS;

        private int[] _MB;
        private int[] _BK;
        private int[] _SB;
        private int[] _BBS;
        private int[] _BCS;

        private GameObject[] tempArray;
        private int numB;


        float distCenter = 300;
        bool withDowntownArea = true;
        float downTownSize = 100;

        public void ClearCity()
        {
            if (!cityMaker)
                cityMaker = GameObject.Find("City-Maker");

            if (cityMaker)
                DestroyImmediate(cityMaker);

        }

        public void GenerateCity(int size, bool withSatteliteCity = false, bool borderFlat = false)
        {

            bool satCity = false;

            if (size == 1)
            {
                // Very Small City
                satCity = GenerateStreetsVerySmall(borderFlat, withSatteliteCity);
            }
            else if (size == 2)
            {
                // Small City
                satCity = GenerateStreetsSmall(borderFlat, withSatteliteCity );
            }
            else if (size == 3)
            {
                // Medium City
                satCity = GenerateStreets(borderFlat, withSatteliteCity);
            }
            else if (size == 4)
            {
                // Large City
                satCity = GenerateStreetsBig(borderFlat, withSatteliteCity);
            }


            if (satCity)
            {
                Transform exitPositipon = CityExitPosition();

                if (exitPositipon != null)
                {

                    int i = (int)Random.Range(1, 10f);

                    GameObject block;

                    switch (i)
                    {
                        case 8:

                            //GenerateStreetsSmall(false, true, 0, -1516);
                            GenerateStreetsVerySmall(false, false, true, 0, -1516);
                            block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position + exitPositipon.forward * 400, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position + exitPositipon.forward * 800, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 7:

                            GenerateStreetsVerySmall(false, false, true, -300, -1516);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 400 + exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 800 + exitPositipon.right * 200, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 6:

                            GenerateStreetsVerySmall(false, false, true, 200, -1516);
                            block = (GameObject)Instantiate(forward400[Random.Range(0, forward400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 400, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 800 - exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 5:

                            GenerateStreetsVerySmall(false, false, true, -100, -1516);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 400 + exitPositipon.right * 100, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 800 + exitPositipon.right * 200, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 4:

                            GenerateStreetsVerySmall(false, false, true, 700, -1316);
                            block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position + exitPositipon.forward * 300 - exitPositipon.right * 300, Quaternion.Euler(0, 270, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 - exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 3:

                            GenerateStreetsVerySmall(false, false, true, 500, -1316);
                            block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position + exitPositipon.forward * 300 - exitPositipon.right * 300, Quaternion.Euler(0, 270, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 - exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        case 2:

                            GenerateStreetsVerySmall(false, false, true, -700, -1316);
                            block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position + exitPositipon.forward * 300 + exitPositipon.right * 300, Quaternion.Euler(0, 90, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardRight400[Random.Range(0, forwardRight400.Length)], exitPositipon.position + exitPositipon.forward * 600 + exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;

                        default:

                            GenerateStreetsVerySmall(false, false, true, -500, -1316);
                            block = (GameObject)Instantiate(right300[Random.Range(0, right300.Length)], exitPositipon.position, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(left300[Random.Range(0, left300.Length)], exitPositipon.position + exitPositipon.forward * 300 + exitPositipon.right * 300, Quaternion.Euler(0, 90, 0), cityMaker.transform);
                            block = (GameObject)Instantiate(forwardLeft400[Random.Range(0, forwardLeft400.Length)], exitPositipon.position + exitPositipon.forward * 600 + exitPositipon.right * 600, Quaternion.Euler(0, 0, 0), cityMaker.transform);
                            break;



                    }

                }
                else
                {
                    Debug.Log("ExitCity gameobject not found");

                }

            }



            DayNight dayNight = FindObjectOfType<DayNight>();
            if (dayNight)
                dayNight.ChangeMaterial();

        }

        private Transform CityExitPosition()
        {

            if (GameObject.Find("ExitCity"))
                return GameObject.Find("ExitCity").transform;
            else
                return null;

        }

        private GameObject InstantiatePrefab (GameObject gameObject, Vector3 pos, Quaternion rot, Transform parent)
        {

            GameObject obj;

#if UNITY_EDITOR
            obj = PrefabUtility.InstantiatePrefab(gameObject, parent) as GameObject;
#else
            obj = Instantiate(gameObject, parent) as GameObject;
#endif
            
            obj.transform.position = pos;
            obj.transform.rotation = rot;

            return obj;

        }

        private bool GenerateStreetsVerySmall(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false, float satteliteCityPositionX = 0, float satteliteCityPositionZ = 0)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();
                cityMaker = new GameObject("City-Maker");
            }

            GameObject block;

            if (!satteliteCity)
                distCenter = 150;

            int nb = 0;

            int le = largeBlocks.Length;
            nb = Random.Range(0, le);

            if (satteliteCity && smallBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(largeBlocks[nb], CityExitPosition().position + new Vector3(satteliteCityPositionX, 0, satteliteCityPositionZ) - new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
                block = (GameObject)Instantiate(largeBlocks[nb], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);


            if ((withSatteliteCity || satteliteCity) && miniBorderWithExitOfCity.Length > 0)
            {
                if (satteliteCity)
                    block = (GameObject)Instantiate(miniBorderWithExitOfCity[Random.Range(0, miniBorderWithExitOfCity.Length)], CityExitPosition().position + new Vector3(satteliteCityPositionX, 0, satteliteCityPositionZ), Quaternion.Euler(0, 180, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(miniBorderWithExitOfCity[Random.Range(0, miniBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(miniBorderFlat[Random.Range(0, miniBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(miniBorder[Random.Range(0, miniBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && miniBorderWithExitOfCity.Length > 0);

        }

        private bool GenerateStreetsSmall(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();

                cityMaker = new GameObject("City-Maker");

            }

            if (!satteliteCity)
                distCenter = 200;

            int nb = 0;

            int le = largeBlocks.Length;
            _largeBlocks = new bool[largeBlocks.Length];

            //Position and Rotation
            Vector3[] ps = new Vector3[3];

            int[] rt = new int[3];

            float s = Random.Range(0, 6f);

            if (s < 3)
            {
                ps[1] = new Vector3(0, 0, 0); rt[1] = 0;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0;
            }
            else
            {
                ps[1] = new Vector3(-150, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(150, 0, 150); rt[2] = 90;
            }


            for (int qt = 1; qt < 3; qt++)
            {

                for (int lp = 0; lp < 100; lp++)
                {
                    nb = Random.Range(0, le);
                    if (!_largeBlocks[nb]) break;
                }
                _largeBlocks[nb] = true;

                if (satteliteCity && smallBorderWithExitOfCity.Length > 0)
                    Instantiate(largeBlocks[nb], ps[qt] + CityExitPosition().position + new Vector3(-0, 0, -1516) - new Vector3(0, 0, 300), Quaternion.Euler(0, rt[qt] + 180, 0), cityMaker.transform);
                else
                    Instantiate(largeBlocks[nb], ps[qt], Quaternion.Euler(0, rt[qt], 0), cityMaker.transform);

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && smallBorderWithExitOfCity.Length > 0)
            {
                if (satteliteCity)
                    block = (GameObject)Instantiate(smallBorderWithExitOfCity[Random.Range(0, smallBorderWithExitOfCity.Length)], CityExitPosition().position + new Vector3(-0, 0, -1516), Quaternion.Euler(0, 180, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(smallBorderWithExitOfCity[Random.Range(0, smallBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);

            }
            else
            {
                if (borderFlat)
                    block = (GameObject)Instantiate(smallBorderFlat[Random.Range(0, smallBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(smallBorder[Random.Range(0, smallBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && smallBorderWithExitOfCity.Length > 0);

        }



        private bool GenerateStreets(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {

                ClearCity();

                cityMaker = new GameObject("City-Maker");
            }

            if (!satteliteCity)
                distCenter = 300;

            int nb = 0;

            int le = largeBlocks.Length;
            _largeBlocks = new bool[largeBlocks.Length];

            //Position and Rotation
            Vector3[] ps = new Vector3[5];

            int[] rt = new int[5];

            float s = Random.Range(0, 6f);

            if (s < 2)
            {

                ps[1] = new Vector3(0, 0, 0); rt[1] = 0;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0;
                ps[3] = new Vector3(450, 0, 150); rt[3] = 90;
                ps[4] = new Vector3(-450, 0, 150); rt[4] = 90;

            }
            else if (s < 3)
            {

                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90;

            }
            else if (s < 4)
            {

                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(300, 0, 0); rt[3] = 0;
                ps[4] = new Vector3(300, 0, 300); rt[4] = 0;

            }
            else
            {

                ps[1] = new Vector3(450, 0, 150); rt[1] = 90;
                ps[2] = new Vector3(150, 0, 150); rt[2] = 90;
                ps[3] = new Vector3(-300, 0, 0); rt[3] = 0;
                ps[4] = new Vector3(-300, 0, 300); rt[4] = 0;

            }


            for (int qt = 1; qt < 5; qt++)
            {

                for (int lp = 0; lp < 100; lp++)
                {
                    nb = Random.Range(0, le);
                    if (!_largeBlocks[nb]) break;
                }
                _largeBlocks[nb] = true;

                Instantiate(largeBlocks[nb], ps[qt], Quaternion.Euler(0, rt[qt], 0), cityMaker.transform);

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && mediumBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(mediumBorderWithExitOfCity[Random.Range(0, mediumBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(mediumBorderFlat[Random.Range(0, mediumBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(mediumBorder[Random.Range(0, mediumBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            }
                

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && mediumBorderWithExitOfCity.Length > 0);

        }


        private bool GenerateStreetsBig(bool borderFlat = false, bool withSatteliteCity = false, bool satteliteCity = false)
        {

            if (satteliteCity && !cityMaker)
                satteliteCity = false;

            if (!satteliteCity)
            {
                ClearCity();
                cityMaker = new GameObject("City-Maker");
            }

            distCenter = 350;

            int nb = 0;

            int le = largeBlocks.Length;
            int lebig = bigLargeBlocks.Length;

            _largeBlocks = new bool[largeBlocks.Length];
            _bigLargeBlocks = new bool[bigLargeBlocks.Length];

            //Position 
            Vector3[] ps = new Vector3[7];

            //Rotation
            int[] rt = new int[7];

            //Type
            int[] tb = new int[7];   // 1->Large - 2->BigLarge

            int qt;

            float s = Random.Range(0, 7f);


            if (s < 3)
            {
                qt = 6;

                ps[1] = new Vector3(0, 0, 0); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(0, 0, 300); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(450, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(-450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;


            }
            else if (s < 3)
            {
                qt = 6;
                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90; tb[1] = 1;
                ps[2] = new Vector3(-150, 0, 150); rt[2] = 90; tb[2] = 1;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;

            }
            else if (s < 4)
            {
                qt = 6;
                ps[1] = new Vector3(-300, 0, 300); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(-300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 150); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(300, 0, 600); rt[6] = 0; tb[6] = 1;


            }
            else if (s < 5)
            {
                qt = 5;
                ps[1] = new Vector3(-300, 0, 0); rt[1] = 0; tb[1] = 1;
                ps[2] = new Vector3(300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(-300, 0, 600); rt[3] = 0; tb[3] = 1;
                ps[4] = new Vector3(300, 0, 600); rt[4] = 0; tb[4] = 1;
                ps[5] = new Vector3(0, 0, 300); rt[5] = 0; tb[5] = 2;



            }
            else
            {
                qt = 6;
                ps[1] = new Vector3(-450, 0, 150); rt[1] = 90; tb[1] = 1;
                ps[2] = new Vector3(300, 0, 0); rt[2] = 0; tb[2] = 1;
                ps[3] = new Vector3(-150, 0, 150); rt[3] = 90; tb[3] = 1;
                ps[4] = new Vector3(450, 0, 450); rt[4] = 90; tb[4] = 1;
                ps[5] = new Vector3(-300, 0, 600); rt[5] = 0; tb[5] = 1;
                ps[6] = new Vector3(150, 0, 450); rt[6] = 90; tb[6] = 1;

            }


            for (int count = 1; count <= qt; count++)
            {

                if (tb[count] == 1)
                {
                    for (int lp = 0; lp < 100; lp++)
                    {
                        nb = Random.Range(0, le);
                        if (!_largeBlocks[nb]) break;
                    }
                    _largeBlocks[nb] = true;

                    Instantiate(largeBlocks[nb], ps[count], Quaternion.Euler(0, rt[count], 0), cityMaker.transform);
                }
                else if (tb[count] == 2)
                {
                    for (int lp = 0; lp < 100; lp++)
                    {
                        nb = Random.Range(0, lebig);
                        if (!_bigLargeBlocks[nb]) break;
                    }
                    _bigLargeBlocks[nb] = true;

                    Instantiate(bigLargeBlocks[nb], ps[count], Quaternion.Euler(0, rt[count], 0), cityMaker.transform);
                }

            }


            GameObject block;

            if ((withSatteliteCity || satteliteCity) && largeBorderWithExitOfCity.Length > 0)
                block = (GameObject)Instantiate(largeBorderWithExitOfCity[Random.Range(0, largeBorderWithExitOfCity.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
            else
            {
                if(borderFlat)
                    block = (GameObject)Instantiate(largeBorderFlat[Random.Range(0, largeBorderFlat.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);
                else
                    block = (GameObject)Instantiate(largeBorder[Random.Range(0, largeBorder.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), cityMaker.transform);

            }
                

            block.transform.SetParent(cityMaker.transform);

            return (withSatteliteCity && largeBorderWithExitOfCity.Length > 0);

        }

        private GameObject pB;

        public void GenerateAllBuildings(bool _withDowntownArea, float _downTownSize)
        {

            downTownSize = _downTownSize;

            withDowntownArea = _withDowntownArea;

            if (withDowntownArea)
            {
                GameObject[] tArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Marcador")).ToArray();

                if (tArray.Length == 1)
                    center = tArray[0].transform.position;
                else
                    center = tArray[Random.Range(1, tArray.Length - 1)].transform.position;

                if (GameObject.Find("DownTownPosition") && Random.Range(1, 10) < 5)
                    center = GameObject.Find("DownTownPosition").transform.position;


            }


            _BB = new int[BB.Length];
            _BC = new int[BC.Length];
            _BR = new int[BR.Length];
            //_DC = new int[DC.Length];
            _EB = new int[EB.Length];
            _EC = new int[EC.Length];
            _MB = new int[MB.Length];
            _BK = new int[BK.Length];
            _SB = new int[SB.Length];

            _EBS = new int[EB.Length];
            _ECS = new int[EC.Length];


            _BBS = new int[BBS.Length];
            _BCS = new int[BCS.Length];

            residential = 0;

            DestroyBuildings();

            GameObject pB = new GameObject();

            nB = 0;

            CreateBuildingsInSuperBlocks();
            CreateBuildingsInBlocks();
            CreateBuildingsInLines();
            CreateBuildingsInDouble();



            //Debug.ClearDeveloperConsole();
            Debug.Log(nB + " buildings were created");


            DestroyImmediate(pB);

            DayNight dayNight = FindObjectOfType<DayNight>();
            if(dayNight)
            {
                dayNight.ChangeMaterial();
            }



        }



        public void CreateBuildingsInLines()
        {


            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Marcador")).ToArray();

            foreach (GameObject lines in tempArray)
            {

                _residential = (residential < 15 && Vector3.Distance(center, lines.transform.position) > 400 && Random.Range(0, 100) < 30);

                foreach (Transform child in lines.transform)
                {

                    if (child.name == "E")
                        CreateBuildingsInCorners(child.gameObject);
                    else if (child.name == "EL")
                    {
                        int ct = 0;
                        do
                        {
                            ct++;
                            if (CreateBuildingsInCorners(child.gameObject, true))
                                break;

                        } while (ct < 300);

                    }
                    else if (child.name.Substring(0, 1) == "S")
                        CreateBuildingsInLine(child.gameObject, 90f, true);
                    else
                        CreateBuildingsInLine(child.gameObject, 90f);

                }

                _residential = false;


            }

        }

        public bool CreateBuildingsInCorners(GameObject child, bool notAnyone = false)
        {

            GameObject pBuilding;

            pB = null;
            int numB = 0;
            int t = 0;
            float pWidth = 0;
            float wComprimento;

            float pScale;
            float remainingMeters;
            GameObject newMarcador;

            float distancia = Vector3.Distance(center, child.transform.position);

            int lp;
            lp = 0;
            int lt = 0;

            float _distCenter = distCenter * (Mathf.Clamp(downTownSize, 50, 200) / 100);

            while (t < 100)
            {

                t++;

                if (distancia < _distCenter && withDowntownArea)
                {

                    do
                    {
                        lp++;
                        lt = 0;
                        do
                        {
                            lt++;
                            numB = Random.Range(0, EC.Length);
                        } while (notAnyone && _ECS[numB] > 0 && lt < 2000);

                        if (_EC[numB] == 0) break;
                        if (lp > 100 && _EC[numB] <= 1) break;
                        if (lp > 150 && _EC[numB] <= 2) break;
                        if (lp > 200 && _EC[numB] <= 3) break;
                        if (lp > 250) break;

                    } while (lp < 300);

                    pWidth = GetWith(EC[numB]);

                    if (pWidth <= 0)
                    {
                        Debug.LogWarning("Error: EC: " + numB);
                        _EC[numB] = 100;
                        return false;
                    }
                    else if (pWidth <= 36.3f)
                    {
                        _EC[numB] += 1;
                        pB = EC[numB];
                        break;
                    }

                }
                else
                {

                    do
                    {
                        lp++;
                        do
                        {
                            lt++;
                            numB = Random.Range(0, EB.Length);
                        } while (notAnyone && _EBS[numB] >= 100 && lt < 2000);

                        if (_EB[numB] == 0) break;
                        if (lp > 100 && _EB[numB] <= 1) break;
                        if (lp > 150 && _EB[numB] <= 2) break;
                        if (lp > 200 && _EB[numB] <= 3) break;
                        if (lp > 250) break;

                    } while (lp < 300);


                    pWidth = GetWith(EB[numB]);

                    if (pWidth <= 0)
                    {
                        Debug.LogWarning("Error: EB: " + numB);
                        _EB[numB] = 100;
                        return false;
                    }
                    else if (pWidth <= 36.3f)
                    {
                        _EB[numB] += 1;
                        pB = EB[numB];
                        break;
                    }

                }



            }



            pBuilding = (GameObject)Instantiate(pB, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            

            if (notAnyone && !TestBaseBuildindCornerOnTheSlope(pBuilding.transform))
            {

                if (distancia < _distCenter && withDowntownArea)
                {
                    _ECS[numB] = 100;
                    _EC[numB] -= 1;
                }
                else
                {
                    _EBS[numB] = 100;
                    _EB[numB] -= 1;
                }


                DestroyImmediate(pBuilding);

                return false;
            }

            pBuilding.name = pBuilding.name;
            pBuilding.transform.SetParent(child.transform);
            pBuilding.transform.localPosition = new Vector3(-(pWidth * 0.5f), 0, 0);
            pBuilding.transform.localRotation = Quaternion.Euler(0, 0, 0);

            nB++;

            // Check space behind the corner building -------------------------------------------------------------------------------------------------------------------
            wComprimento = GetHeight(pB);
            if (wComprimento < 29.9f)
            {

                newMarcador = new GameObject("Marcador");

                newMarcador.transform.SetParent(child.transform);
                newMarcador.transform.localPosition = new Vector3(0, 0, -36);
                newMarcador.transform.localRotation = Quaternion.Euler(0, 0, 0);
                newMarcador.name = (36 - wComprimento).ToString();
                CreateBuildingsInLine(newMarcador, 90);

            }
            else
            {
                remainingMeters = 36 - wComprimento;
                pScale = 1 + (remainingMeters / wComprimento);
                pBuilding.transform.localScale = new Vector3(1, 1, pScale);

            }


            // Check space on the corner building -------------------------------------------------------------------------------------------------------------------


            if (pWidth < 29.9f)
            {

                newMarcador = new GameObject("Marcador");



                newMarcador.transform.SetParent(child.transform);
                newMarcador.transform.localPosition = new Vector3(-pWidth, 0, 0);
                newMarcador.transform.localRotation = Quaternion.Euler(0, 270, 0);
                newMarcador.name = (36 - pWidth).ToString();
                CreateBuildingsInLine(newMarcador, 90);

            }
            else
            {

                remainingMeters = 36 - pWidth;
                pScale = 1 + (remainingMeters / pWidth);
                pBuilding.transform.localScale = new Vector3(pScale, 1, 1);

            }

            return true;

        }

        bool TestBaseBuildindCornerOnTheSlope(Transform buildingCornerOnTheSlope)
        {
            return (buildingCornerOnTheSlope.Find("Base-Corner-0-Collider") || buildingCornerOnTheSlope.Find("Base-Corner-03-Collider") || buildingCornerOnTheSlope.Find("Base-Corner-06-Collider"));
        }

        int RandRotation()
        {
            int r = 0;
            int i = Random.Range(0, 4);
            if (i == 3) r = 180;
            else if (i == 2) r = 90;
            else if (i == 1) r = 270;
            else r = 0;

            return r;


        }


        public void CreateBuildingsInBlocks()
        {

            int numB = 0;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Blocks")).ToArray();

            foreach (GameObject bks in tempArray)
            {

                foreach (Transform bk in bks.transform)
                {

                    if (Random.Range(0, 20) > 5)
                    {

                        int lp = 0;
                        do
                        {
                            lp++;
                            numB = Random.Range(0, BK.Length);
                            if (_BK[numB] == 0) break;
                            if (lp > 125 && _BK[numB] <= 1) break;
                            if (lp > 150 && _BK[numB] <= 2) break;
                            if (lp > 200 && _BK[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        _BK[numB] += 1;


                        Instantiate(BK[numB], bk.position, bk.rotation, bk);
                        
                        nB++;

                    }
                    else
                    {

                        for (int i = 1; i <= 4; i++)
                        {
                            GameObject nc = new GameObject("E");
                            nc.transform.SetParent(bk);
                            if (i == 1)
                            {
                                nc.transform.localPosition = new Vector3(-36, 0, -36);
                                nc.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            }
                            if (i == 2)
                            {
                                nc.transform.localPosition = new Vector3(-36, 0, 36);
                                nc.transform.localRotation = Quaternion.Euler(0, 270, 0);
                            }
                            if (i == 3)
                            {
                                nc.transform.localPosition = new Vector3(36, 0, 36);
                                nc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            if (i == 4)
                            {
                                nc.transform.localPosition = new Vector3(36, 0, -36);
                                nc.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            }
                            CreateBuildingsInCorners(nc);

                        }
                    }


                }

            }

        }

        public void CreateBuildingsInSuperBlocks()
        {

            int numB = 0;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("SuperBlocks")).ToArray();

            foreach (GameObject bks in tempArray)
            {

                foreach (Transform bk in bks.transform)
                {


                    int lp = 0;
                    do
                    {
                        lp++;
                        numB = Random.Range(0, SB.Length);
                        if (_SB[numB] == 0) break;
                        if (lp > 125 && _SB[numB] <= 1) break;
                        if (lp > 150 && _SB[numB] <= 2) break;
                        if (lp > 200 && _SB[numB] <= 3) break;
                        if (lp > 250) break;
                    } while (lp < 300);

                    _SB[numB] += 1;

                    Instantiate(SB[numB], bk.position, bk.rotation, bk);
                    
                    nB++;



                }

            }

        }

        private void CreateBuildingsInLine(GameObject line, float angulo, bool slope = false)
        {


            int index = -1;
            GameObject[] pBuilding;
            pBuilding = new GameObject[50];

            float limit;
            string _name = line.name;

            _name = (slope) ? line.name.Substring(1) : line.name;

            if (_name.Contains("."))
                limit = float.Parse(_name.Split('.')[0]) + float.Parse(_name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, _name.Split('.')[1].Length));
            else
                limit = float.Parse(_name);

            float init = 0;
            float pWidth = 0;

            int tt = 0;
            int t;

            int lp;


            float distancia = Vector3.Distance(center, line.transform.position);

            float _distCenter = distCenter * (Mathf.Clamp(downTownSize, 50, 200) / 100);

            while (tt < 100)
            {

                tt++;
                t = 0;


                lp = 0;
                while (t < 200 && init <= limit - 4)
                {

                    t++;

                    if (slope)
                    {
                        if (distancia < _distCenter && withDowntownArea)
                        {
                            do
                            {
                                lp++;
                                numB = Random.Range(0, BCS.Length);
                                if (_BCS[numB] == 0) break;
                                if (lp > 125 && _BCS[numB] <= 1) break;
                                if (lp > 150 && _BCS[numB] <= 2) break;
                                if (lp > 200 && _BCS[numB] <= 3) break;
                                if (lp > 250) break;

                            } while (lp < 300);

                            pWidth = GetWith(BCS[numB]);

                            if (pWidth > 0)
                                if ((init + pWidth) <= (limit + 4))
                                {
                                    pB = BCS[numB];
                                    _BCS[numB] += 1;
                                    break;
                                }
                        }
                        else
                        {

                            do
                            {
                                lp++;
                                numB = Random.Range(0, BBS.Length);
                                if (_BBS[numB] == 0) break;
                                if (lp > 125 && _BBS[numB] <= 1) break;
                                if (lp > 150 && _BBS[numB] <= 2) break;
                                if (lp > 200 && _BBS[numB] <= 3) break;
                                if (lp > 250) break;

                            } while (lp < 300);

                            pWidth = GetWith(BBS[numB]);

                            if (pWidth > 0)
                                if ((init + pWidth) <= (limit + 4))
                                {
                                    pB = BBS[numB];
                                    _BBS[numB] += 1;
                                    break;
                                }

                        }

                    }
                    else if (distancia < _distCenter && withDowntownArea)
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BC.Length);
                            if (_BC[numB] == 0) break;
                            if (lp > 125 && _BC[numB] <= 1) break;
                            if (lp > 150 && _BC[numB] <= 2) break;
                            if (lp > 200 && _BC[numB] <= 3) break;
                            if (lp > 250) break;

                        } while (lp < 300);

                        pWidth = GetWith(BC[numB]);

                        if (pWidth > 0)
                            if ((init + pWidth) <= (limit + 4))
                            {
                                pB = BC[numB];
                                _BC[numB] += 1;
                                break;
                            }

                    }
                    else if (_residential)
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BR.Length);
                            if (_BR[numB] == 0) break;
                            if (lp > 100 && _BR[numB] <= 1) break;
                            if (lp > 150 && _BR[numB] <= 2) break;
                            if (lp > 200 && _BR[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        pWidth = GetWith(BR[numB]);

                        if (pWidth <= 0) { Debug.LogWarning("Error: BR: " + numB); _BR[numB] += 1; }
                        else
                        if ((init + pWidth) <= (limit + 4))
                        {
                            pB = BR[numB];
                            _BR[numB] += 1;
                            residential += 1;
                            break;
                        }
                    }
                    else
                    {

                        do
                        {
                            lp++;
                            numB = Random.Range(0, BB.Length);
                            if (_BB[numB] == 0) break;
                            if (lp > 100 && _BB[numB] <= 1) break;
                            if (lp > 150 && _BB[numB] <= 2) break;
                            if (lp > 200 && _BB[numB] <= 3) break;
                            if (lp > 250) break;
                        } while (lp < 300);

                        pWidth = GetWith(BB[numB]);

                        if (pWidth <= 0) { Debug.LogWarning("Error: BB: " + numB); _BB[numB] += 1; }
                        if ((init + pWidth) <= (limit + 4))
                        {
                            pB = BB[numB];
                            _BB[numB] += 1;
                            break;
                        }

                    }


                }


                if (t >= 200 || init > limit - 4)
                {
                    // Didn't find one that fits in the remaining space

                    AdjustsWidth(pBuilding, index + 1, limit - init, 0, slope);
                    break;

                }
                else
                {

                    index++;


                    nB++;

                    //pBuilding[index].name = pBuilding[index].name;

                    pBuilding[index] = (GameObject)Instantiate(pB, new Vector3(0, 0, init + (pWidth * 0.5f)), Quaternion.Euler(0, angulo, 0), line.transform);
                    

                    pBuilding[index].transform.SetParent(line.transform);
                    pBuilding[index].transform.localPosition = new Vector3(0, 0, init + (pWidth * 0.5f));
                    pBuilding[index].transform.localRotation = Quaternion.Euler(0, angulo, 0);

                    init += pWidth;

                    if (init > limit - 6)
                    {
                        AdjustsWidth(pBuilding, index + 1, limit - init, 0, slope);
                        break;
                    }

                }



            }



        }


        private float GetY(Transform pos, float width)
        {

            RaycastHit hit;

            Vector3 pp = pos.transform.position + pos.transform.forward * 2 + pos.transform.up * 20;

            float l = 20;
            float r = 20;

            if (Physics.Raycast(pp + pos.transform.right * width, Vector3.down, out hit, 40))
                r = hit.distance;

            if (Physics.Raycast(pp - (pos.transform.right * width), Vector3.down, out hit, 40))
                l = hit.distance;

            return (pos.transform.localPosition.y + 20) - ((r < l) ? r : l);

        }

        private void CreateBuildingsInDoubleLine(GameObject line)
        {

            int index = -1;
            GameObject[] pBuilding;
            pBuilding = new GameObject[20];

            float limit;
            string _name = line.name;

            if (_name.Contains("."))
                limit = float.Parse(_name.Split('.')[0]) + float.Parse(_name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, _name.Split('.')[1].Length));
            else
                limit = float.Parse(_name);


            float init = 0;
            float pWidth = 0;

            int tt = 0;
            int t;
            int lp;

            while (tt < 100)
            {

                tt++;
                t = 0;

                lp = 0;

                while (t < 200 && init <= limit - 4)
                {

                    t++;

                    do
                    {
                        lp++;
                        numB = Random.Range(0, MB.Length);
                        if (_MB[numB] == 0) break;
                        if (lp > 100 && _MB[numB] <= 1) break;
                        if (lp > 150 && _MB[numB] <= 2) break;
                        if (lp > 200) break;
                    } while (lp < 300);

                    pWidth = GetWith(MB[numB]);


                    if (pWidth <= 0) { Debug.LogWarning("Error: MB: " + numB); _MB[numB] += 1; }
                    else
                    if ((init + pWidth) <= (limit + 4))
                    {
                        _MB[numB] += 1;
                        break;
                    }

                }

                if (t >= 200 || init > limit - 4)
                {
                    AdjustsWidth(pBuilding, index + 1, (limit - init), 0);
                    break;

                }
                else
                {

                    index++;

                    pBuilding[index] = (GameObject)Instantiate(MB[numB], new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0), line.transform);
                    

                    nB++;

                    pBuilding[index].name = "building";
                    pBuilding[index].transform.SetParent(line.transform);
                    pBuilding[index].transform.localPosition = new Vector3(0, 0, (init + (pWidth * 0.5f)));
                    pBuilding[index].transform.localRotation = Quaternion.Euler(0, 90, 0);

                    init += pWidth;

                    if (init > limit - 6)
                    {
                        AdjustsWidth(pBuilding, index + 1, (limit - init), 0);
                    }

                }


            }

        }

        private void CreateBuildingsInDouble()
        {
            float limit;

            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("Double")).ToArray();

            GameObject DB;
            GameObject mc2;
            GameObject mc;


            foreach (GameObject dbCross in tempArray)
            {

                foreach (Transform line in dbCross.transform)
                {

                    if (line.name.Contains("."))
                        limit = float.Parse(line.name.Split('.')[0]) + float.Parse(line.name.Split('.')[1]) / float.Parse("1" + "0000000".Substring(0, line.name.Split('.')[1].Length));
                    else
                        limit = float.Parse(line.name);



                    if (Random.Range(0, 10) < 5)
                    {
                        //Bloks

                        float wl;
                        float wl2;

                        do
                        {
                            numB = Random.Range(0, DC.Length);
                            wl = GetHeight(DC[numB]);
                        } while (wl > limit / 2);

                        GameObject e = (GameObject)Instantiate(DC[numB], line.transform.position, line.transform.rotation, line.transform);
                        

                        nB++;

                        do
                        {
                            numB = Random.Range(0, DC.Length);
                            wl2 = GetHeight(DC[numB]);
                        } while (wl2 > limit - (wl + 26));

                        e = (GameObject)Instantiate(DC[numB], line.transform.position, line.rotation, line.transform);


                        e.transform.SetParent(line.transform);
                        e.transform.localPosition = new Vector3(0, 0, -limit);
                        e.transform.localRotation = Quaternion.Euler(0, 180, 0);

                        DB = new GameObject("" + ((limit - wl - wl2)));
                        DB.transform.SetParent(line.transform);
                        DB.transform.localPosition = new Vector3(0, 0, -(limit - wl2));
                        DB.transform.localRotation = Quaternion.Euler(0, 0, 0);

                        DB.name = "" + ((limit - wl - wl2));

                        CreateBuildingsInDoubleLine(DB);

                    }
                    else
                    {
                        //Lines and Corners

                        mc = new GameObject("Marcador");
                        mc.transform.SetParent(line);
                        mc.transform.localPosition = new Vector3(0, 0, 0);
                        mc.transform.localRotation = Quaternion.Euler(0, 0, 0);


                        for (int i = 1; i <= 4; i++)
                        {
                            mc2 = new GameObject("E");
                            mc2.transform.SetParent(mc.transform);

                            if (i == 1)
                            {
                                mc2.transform.localPosition = new Vector3(36, 0, -limit);
                                mc2.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            }
                            if (i == 2)
                            {
                                mc2.transform.localPosition = new Vector3(36, 0, 0);
                                mc2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            if (i == 3)
                            {
                                mc2.transform.localPosition = new Vector3(-36, 0, 0);
                                mc2.transform.localRotation = Quaternion.Euler(0, 270, 0);
                            }
                            if (i == 4)
                            {
                                mc2.transform.localPosition = new Vector3(-36, 0, -limit);
                                mc2.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            }

                            CreateBuildingsInCorners(mc2);

                        }

                        mc2 = new GameObject("" + (limit - 72));
                        mc2.transform.SetParent(mc.transform);
                        mc2.transform.localPosition = new Vector3(-36, 0.001f, -36);
                        mc2.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        CreateBuildingsInLine(mc2, 90f);

                        mc2 = new GameObject("" + (limit - 72));
                        mc2.transform.SetParent(mc.transform);
                        mc2.transform.localPosition = new Vector3(36, 0.001f, -(limit - 36));
                        mc2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        CreateBuildingsInLine(mc2, 90f);

                    }




                }



            }


        }


        private void AdjustsWidth(GameObject[] tBuildings, int quantity, float remainingMeters, float init, bool slope = false)
        {

            if (remainingMeters == 0)
                return;

            float ajuste = remainingMeters / quantity;

            float zInit = init;
            float pWidth;
            float pScale;
            float gw;


            for (int i = 0; i < quantity; i++)
            {

                gw = GetWith(tBuildings[i]);

                if (gw > 0)
                {
                    pScale = 1 + (ajuste / gw);
                    pWidth = gw + ajuste;

                    tBuildings[i].transform.localPosition = new Vector3(tBuildings[i].transform.localPosition.x, tBuildings[i].transform.localPosition.y, zInit + (pWidth * 0.5f));
                    tBuildings[i].transform.localScale = new Vector3(pScale, 1, 1);
                    zInit += pWidth;


                    if (slope)
                    {
                        float p;

                        p = GetY(tBuildings[i].transform, (gw * pScale) * 0.5f);
                        tBuildings[i].transform.position += new Vector3(0, p, 0);


                    }


                }

            }

        }


        private float GetWith(GameObject building)
        {

            if (!building)
                return 0;


            if (building.transform.GetComponent<MeshFilter>() != null)
            {

                if (building.transform.GetComponent<MeshFilter>().sharedMesh == null)
                {
                    Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                    //return 0;
                }


                return building.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            }
            else
            {
                Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                return 0;
            }
        }

        private float GetHeight(GameObject building)
        {

            if (building.GetComponent<MeshFilter>() != null)
                return building.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
            else
            {
                Debug.LogError("Error:  " + building.name + " does not have a mesh renderer at the root. The prefab must be the floor/base mesh. I nside it you place the building. More info: https://youtu.be/kVrWir_WjNY");
                return 0;
            }

        }


        public void DestroyBuildings()
        {

            DestryObjetcs("Marcador");
            DestryObjetcs("Blocks");
            DestryObjetcs("SuperBlocks");
            DestryObjetcs("Double");

        }


        private void DestryObjetcs(string tag)
        {
            tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == (tag)).ToArray();

            foreach (GameObject objt in tempArray)
                foreach (Transform child in objt.transform)
                    for (int k = child.childCount - 1; k >= 0; k--)
                        DestroyImmediate(child.GetChild(k).gameObject);


        }



    }
}