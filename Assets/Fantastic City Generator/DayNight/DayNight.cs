using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;


public class DayNight : MonoBehaviour
{
    //  In the 2 fields below, only the materials that will be alternated in the day/night exchange are registered
    //  When adding your buildings(which will have their own materials), you can register the day and night versions of the materials here.
    //  The index of the daytime version of the material must match the index of the nighttime version of the material
    //  Example: When switching to night scene, materialDay[1] will be replaced by materialNight[1]
    //  (Materials that will be used both night and day do not need to be here)
    public Material[] materialDay;   // Add materials that are only used in the day scene, and are substituted in the night scene
    public Material[] materialNight; // Add night scene materials that will replace day scene materials. (The sequence must be respected)


    //Here are the skyboxes for day and night.
    //You can replace with other skyboxes
    public Material skyBoxDay;
    public Material skyBoxNight;

    //Don't forget to add the Directional Light here
    public Light directionalLight;

    [HideInInspector]
    public bool isNight;

    [HideInInspector]
    public bool isMoonLight;

    [HideInInspector]
    public bool isSpotLights;

    [HideInInspector]
    public bool night;

    [HideInInspector]
    public bool moonLight;

    [HideInInspector]
    public bool spotLights;

    [HideInInspector]
    public float intenseMoonLight = 0.2f;

    [HideInInspector]
    public float _intenseMoonLight;

    [HideInInspector]
    public float intenseSunLight = 1f;

    [HideInInspector]
    public float _intenseSunLight;

    [HideInInspector]
    public Color skyColorDay = new Color(0.74f, 0.62f, 0.60f);
    [HideInInspector]
    public Color equatorColorDay = new Color(0.74f, 0.74f, 0.74f);


    [HideInInspector]
    public Color _skyColorDay;
    [HideInInspector]
    public Color _equatorColorDay;


    [HideInInspector]
    public Color skyColorNight = new Color(0.78f, 0.72f , 0.72f);
    [HideInInspector]
    public Color equatorColorNight = new Color(0.16f, 0.16f, 0.16f);

    [HideInInspector]
    public Color _skyColorNight;
    [HideInInspector]
    public Color _equatorColorNight;

    [HideInInspector]
    public Color sunLightColor;
    [HideInInspector]
    public Color _sunLightColor;

    [HideInInspector]
    public Color moonLightColor;
    [HideInInspector]
    public Color _moonLightColor;


    public void ChangeMaterial()
    {
       
        //Switching the skyboxes according to day/ night
        RenderSettings.skybox = (isNight) ? skyBoxNight : skyBoxDay;

        /*
        Setting the ambient (gradient) light to fit day/night
        These are properties of the "Lighting" window and Directional Light
        */
        UpdateColor();


        /*
          Substituting Night materials for Day materials (or vice versa) in all Mesh Renders within City-Maker
          Only materials that have been added in "materialDay" and "materialNight" Array
        */

        GameObject GmObj = GameObject.Find("City-Maker");

        if (GmObj == null) return;

        Renderer[] children = GmObj.GetComponentsInChildren<Renderer>();

        Material[] myMaterials;

        for (int i = 0; i < children.Length; i++)
        {
            myMaterials = children[i].GetComponent<Renderer>().sharedMaterials;

            for (int m = 0; m < myMaterials.Length; m++)
            {
                for (int mt = 0; mt < materialDay.Length; mt++)
                if (isNight)
                {
                    if(myMaterials[m] == materialDay[mt])
                        myMaterials[m] = materialNight[mt];

                }
                else
                {
                    if (myMaterials[m] == materialNight[mt])
                        myMaterials[m] = materialDay[mt];
                }


                children[i].GetComponent<MeshRenderer>().sharedMaterials = myMaterials;
            }


        }

        //Configuring the Directional Light as it is day or night (sun/moon)
        SetDirectionalLight();

        //Toggles street lamp lights on/off
        SetStreetLights();


    }

    public void UpdateColor()
    {

        /*
        Setting the ambient (gradient) light to fit day/night
        These are properties of the "Lighting" window and Directional Light
        */



        if (isNight)
        {
            
            //During the Night

            if (directionalLight)
                directionalLight.GetComponent<Light>().color = moonLightColor;

            RenderSettings.ambientMode = AmbientMode.Trilight;

            RenderSettings.ambientSkyColor = skyColorNight;  // Floor color/brightness (any face up) - no moonlight

            RenderSettings.ambientEquatorColor = equatorColorNight;  // Wall color/ luminosity(any side facing)
            RenderSettings.ambientGroundColor = new Color(0.07f, 0.07f, 0.07f);  // Ceiling color/brightness (any face down)
        }
        else
        {

            if (directionalLight)
                directionalLight.GetComponent<Light>().color = sunLightColor;

            //During the day
            RenderSettings.ambientMode = AmbientMode.Trilight;


            RenderSettings.ambientSkyColor = skyColorDay;     // Floor color/brightness (any face up) 
            RenderSettings.ambientEquatorColor = equatorColorDay; // Wall color/ luminosity(any side facing)
            RenderSettings.ambientGroundColor = new Color(0.4f, 0.4f, 0.4f); ;   // Ceiling color/brightness (any face down)

        }
    }

    public void SetDirectionalLight()
    {
        if (directionalLight)
        {
            //Directional Light activated only during the day, or if it is night with moonlight
            directionalLight.GetComponent<Light>().enabled = (!isNight || isMoonLight);

            //Directional Light intensity, depending on day or night
            directionalLight.intensity = (isNight) ? intenseMoonLight / 100: intenseSunLight / 100;


        }
    }

    //Toggles street lamp lights on/off
    public void SetStreetLights()
    {
        GameObject[] tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("_LightV")).ToArray();

        foreach (GameObject lines in tempArray)
        {
            lines.GetComponent<MeshRenderer>().enabled = isNight;
            if (lines.transform.GetChild(0))
            {
                lines.transform.GetChild(0).GetComponent<Light>().enabled = (isSpotLights && isNight);
            }
        }

        tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == ("_Spot_Light")).ToArray();

        foreach (GameObject lines in tempArray)
        {
            lines.GetComponent<Light>().enabled = (isSpotLights && isNight);
        }

    }


}
