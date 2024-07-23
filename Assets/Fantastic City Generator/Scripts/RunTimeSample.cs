using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FCG;
public class RunTimeSample : MonoBehaviour
{

    public GameObject cg;

    private CityGenerator generator;
    private TrafficSystem trafficSystem;

    private bool withDownTownArea = true;
    private bool rightHand = true;


    void Awake()
    {

        generator = cg.GetComponent<CityGenerator>();

    }
    public void GenerateCityAtRuntime(int citySize)
    {

        Destroy(GameObject.Find("CarContainer"));

        generator = cg.GetComponent<CityGenerator>();

        generator.GenerateCity(citySize, false,false); // (city size:  1 , 2, 3 or 4) 


    }

    public void WithDownTownArea(bool value)
    {
        withDownTownArea = value;
    }
    public void RightHand(bool value)
    {
        rightHand = value;
    }
    

    public void GenerateBuildings()
    {
        float downTownSize = 100;
        generator.GenerateAllBuildings(withDownTownArea, downTownSize); // (skyscrappers: true or false)

    }


    public void AddTrafficSystem()
    {

        trafficSystem = FindObjectOfType<TrafficSystem>();

        if (trafficSystem) 
        { 

            trafficSystem.LoadCars((rightHand) ? 0 : 1);

            Debug.LogWarning("Move the camera to the streets so that vehicles are generated around it");
    
        } else
            Debug.LogError("Traffic System prefab not found in Hierarchy");


    }


}
