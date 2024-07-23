using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using FCG;

public class FCityGenerator : EditorWindow
{

    private CityGenerator cityGenerator;

    private bool generateLightmapUVs = false;
    private bool withDowntownArea = true;
    private float downTownSize = 100;

    private bool withSatteliteCity = false;
    private bool borderFlat = false;


    private int trafficLightHand = 0;
    private string[] selStrings = { "Right Hand", "Left Hand" };
    private bool japanTrafficLight = false;


    [MenuItem("Window/Fantastic City Generator")]
    static void Init()
    {

        FCityGenerator window = (FCityGenerator)EditorWindow.GetWindow(typeof(FCityGenerator));

        window.Show();

    }

    int enableUpdate = 0;

#if UNITY_EDITOR
    void Update()
    {

        if (enableUpdate == 0) return;

        enableUpdate++;

        if (enableUpdate <= 5)
            HideLadders();

        if (enableUpdate >= 5)
            enableUpdate = 0;

    }
#endif

    public void LoadAssets(bool force = false)
    {
        cityGenerator = null;

        if (!cityGenerator)
            cityGenerator = (CityGenerator)AssetDatabase.LoadAssetAtPath("Assets/Fantastic City Generator/Generate.prefab", (typeof(CityGenerator)));

        if (!cityGenerator)
        {
            Debug.LogError("Generate.prefab was not found/Loaded in 'Assets/Fantastic City Generator'");
            return;
        }

        string[] s;

        //BB - Street buildings in suburban areas (not in the corner)
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BB", "*.prefab");
        if (force || cityGenerator.BB == null || cityGenerator.BB.Length != s.Length)
            cityGenerator.BB = LoadAssets_sub(s);

        //BC - Down Town Buildings(Not in the corner)
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BC", "*.prefab");
        if (force || cityGenerator.BC == null || cityGenerator.BC.Length != s.Length)
            cityGenerator.BC = LoadAssets_sub(s);

        //BK - Buildings that occupy an entire block
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BK", "*.prefab");
        if (force || cityGenerator.BK == null || cityGenerator.BK.Length != s.Length)
            cityGenerator.BK = LoadAssets_sub(s);

        //BR - Residential buildings in suburban areas (not in the corner)
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BR", "*.prefab");
        if (force || cityGenerator.BR == null || cityGenerator.BR.Length != s.Length)
            cityGenerator.BR = LoadAssets_sub(s);

        //DC - Corner buildings that occupy both sides of the block
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/DC", "*.prefab");
        if (force || cityGenerator.DC == null || cityGenerator.DC.Length != s.Length)
            cityGenerator.DC = LoadAssets_sub(s);

        //EB - Corner buildings in suburban areas
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/EB", "*.prefab");
        if (force || cityGenerator.EB == null || cityGenerator.EB.Length != s.Length)
            cityGenerator.EB = LoadAssets_sub(s);

        //EC - Down Town Corner Buildings 
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/EC", "*.prefab");
        if (force || cityGenerator.EC == null || cityGenerator.EC.Length != s.Length)
            cityGenerator.EC = LoadAssets_sub(s);

        //MB - Buildings that occupy both sides of the block
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/MB", "*.prefab");
        if (force || cityGenerator.MB == null || cityGenerator.MB.Length != s.Length)
            cityGenerator.MB = LoadAssets_sub(s);

        //SB - Large buildings that occupy larger blocks
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/SB", "*.prefab");
        if (force || cityGenerator.SB == null || cityGenerator.SB.Length != s.Length)
            cityGenerator.SB = LoadAssets_sub(s);

        //BBS - Buildings on slopes (neighborhood)
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BBS", "*.prefab");
        if (force || cityGenerator.BBS == null || cityGenerator.BBS.Length != s.Length)
            cityGenerator.BBS = LoadAssets_sub(s);

        //BCS - Down Town Buildings on slopes
        s = System.IO.Directory.GetFiles("Assets/Fantastic City Generator/Buildings/Prefabs/BCS", "*.prefab");
        if (force || cityGenerator.BCS == null || cityGenerator.BCS.Length != s.Length)
            cityGenerator.BCS = LoadAssets_sub(s);

    }



    private GameObject[] LoadAssets_sub(string[] s)
    {

        int i = s.Length;
        GameObject[] g = new GameObject[i];

        for (int h = 0; h < i; h++)
            g[h] = AssetDatabase.LoadAssetAtPath(s[h], typeof(GameObject)) as GameObject;

        if (g == null)
            Debug.LogError("Error in LoadAssets");

        return g;

    }

    private void GenerateCity(int size, bool borderFlat = false)
    {

        LoadAssets();
        
        cityGenerator.GenerateCity(size, withSatteliteCity, borderFlat);

        if (trafficSystem)
        {
            InverseCarDirection((trafficLightHand == 1 && japanTrafficLight) ? 2 : trafficLightHand);

            trafficSystem.UpdateAllWayPoints();

        }


        DestroyImmediate(GameObject.Find("CarContainer"));


    }



    public void HideLadders()
    {

        RaycastHit hit;

        GameObject[] tempArray = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == "RayCast-HideLadder").ToArray();
        foreach (GameObject ray in tempArray)
        {

            if (Physics.Raycast(ray.transform.position, ray.transform.forward, out hit, 1.5f))
                ray.transform.GetChild(0).gameObject.SetActive(false);
            else
                ray.transform.GetChild(0).gameObject.SetActive(true);

        }


    }


    void OnGUI()
    {

        GUILayout.Space(10);

        GUILayout.Label("Fantastic City Generator", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (!cityGenerator)
            cityGenerator = (CityGenerator)AssetDatabase.LoadAssetAtPath("Assets/Fantastic City Generator/Generate.prefab", (typeof(CityGenerator)));

        if (!cityGenerator)
            Debug.LogError("Generate.prefab was not found in 'Assets/Fantastic City Generator'");

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginVertical("box");

        GUILayout.Space(5);
        GUILayout.Label(new GUIContent("Generate Streets", "Make City"));

        GUILayout.Space(5);

        GUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Small"))
            GenerateCity(1, borderFlat);


        if (GUILayout.Button("Medium"))
            GenerateCity(2, borderFlat);

        if (GUILayout.Button("Large"))
            GenerateCity(3, borderFlat);

        if (GUILayout.Button("Very Large"))
            GenerateCity(4, borderFlat);


        GUILayout.Space(5);


        GUILayout.EndHorizontal();


        withSatteliteCity = GUILayout.Toggle(withSatteliteCity, "With Sattelite City", GUILayout.Width(240));

        if (!withSatteliteCity)
        {
            GUILayout.Space(10);
            borderFlat = GUILayout.Toggle(borderFlat, "Border Flat", GUILayout.Width(240));
        }

        GUILayout.Space(10);


        if (GUILayout.Button("Clear Streets "))
        {
            cityGenerator.ClearCity();
        }

        GUILayout.Space(10);

        GUILayout.EndVertical();

        GUILayout.Space(10);



        GUILayout.BeginVertical("box");

        GUILayout.Space(5);

        GUILayout.Label(new GUIContent("Buildings", "Make or Clear Buildings"));

        GUILayout.Space(5);

        GUILayout.BeginHorizontal("box");


        GUILayout.Space(5);

        if (GUILayout.Button("Generate Buildings"))
        {
            if (!GameObject.Find("Marcador")) return;

            LoadAssets(true);

            cityGenerator.GenerateAllBuildings(withDowntownArea, downTownSize);
            enableUpdate = 1;

        }


        if (GUILayout.Button("Clear Buildings"))
        {
            if (!GameObject.Find("Marcador")) return;
            
            
            cityGenerator.DestroyBuildings();
            //DestroyImmediate(GameObject.Find("CarContainer"));
        }






        GUILayout.EndHorizontal();

        withDowntownArea = GUILayout.Toggle(withDowntownArea, "With Downtown Area?", GUILayout.Width(240));

        if (withDowntownArea)
        {
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("DownTown Size:", "DownTown Size"));
            downTownSize = EditorGUILayout.Slider(downTownSize, 50, 200);
            GUILayout.Space(10);
        }

        GUILayout.EndVertical();




        GUILayout.Space(10);



        GUILayout.BeginVertical("box");

        GUILayout.Space(5);

        GUILayout.Label(new GUIContent("Traffic System", "Make or Clear Traffic System"));

        GUILayout.Space(5);


        GUILayout.BeginHorizontal("box");

        GUILayout.Space(5);

        if (GUILayout.Button("Add Traffic System"))
        {
            AddVehicles(trafficLightHand);
        }


        if (GUILayout.Button("Remove Traffic System"))
        {

            DestroyImmediate(GameObject.FindObjectOfType<TrafficSystem>().gameObject);
            DestroyImmediate(GameObject.Find("CarContainer"));
        }

        GUILayout.Space(5);

        GUILayout.EndHorizontal();

        GUILayout.Space(5);


        GUILayout.Space(5);

        GUILayout.BeginVertical("box");
        GUILayout.Label(new GUIContent("Traffic Hand", "Hand Right/Left"));
        int rh = trafficLightHand;
        trafficLightHand = GUILayout.SelectionGrid(trafficLightHand, selStrings, 2);
        GUILayout.EndVertical();

        bool japanTL = japanTrafficLight;

        if (trafficLightHand != 0)
        {
            japanTrafficLight = GUILayout.Toggle(japanTrafficLight, "Japan Traffic Light (blue)", GUILayout.Width(240));
        }


        if (rh != trafficLightHand || japanTL != japanTrafficLight)
        {
            rh = trafficLightHand;
            japanTL = japanTrafficLight;

            if (GameObject.Find("CarContainer"))
                AddVehicles((trafficLightHand == 1 && japanTrafficLight) ? 2 : trafficLightHand);
            else
                InverseCarDirection((trafficLightHand == 1 && japanTrafficLight) ? 2 : trafficLightHand);

        }


        GUILayout.EndVertical();


        GUILayout.Space(10);

        GUILayout.BeginVertical("box");


        if (GUILayout.Button("Combine Meshes"))
        {


            if (!GameObject.Find("Marcador")) return;


            //It is necessary to remove LODs from buildings before combining meshes
            if (!EditorUtility.DisplayDialog("Mesh combine",
                "Mesh combine the buildings will remove the LODs.\n\nDo you still want to continue? ", "Yes", "No"))
                return;

            float vertexCount = 0;
            float tt;
            GameObject module;
            GameObject[] my_Modules;

            my_Modules = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == "Marcador").ToArray();

            tt = my_Modules.Length;

            vertexCount = 0;

            for (int i = 0; i < tt; i++)
            {

                vertexCount = 0;

                module = my_Modules[i];

                GameObject newBlock = new GameObject("_block");
                newBlock.transform.position = module.transform.position;
                newBlock.transform.rotation = module.transform.rotation;
                newBlock.transform.parent = module.transform.parent;

                foreach (Transform child in module.transform)
                {  // E1, E2, 100

                    Component[] temp = child.GetComponentsInChildren(typeof(MeshFilter));

                    //Remove LODs from Buildings before Combine Meshes
                    foreach (MeshFilter currentChild in temp)
                        if (currentChild.gameObject.name.Contains("_LOD"))
                            DestroyImmediate(currentChild.gameObject);

                    temp = child.GetComponentsInChildren(typeof(MeshFilter));

                    foreach (MeshFilter currentChild in temp)
                    {

                        vertexCount += currentChild.sharedMesh.vertexCount;
                        if (vertexCount > 50000)
                        {
                            vertexCount = 0;
                            newBlock = new GameObject("_block");
                            newBlock.transform.position = module.transform.position;
                            newBlock.transform.rotation = module.transform.rotation;
                            newBlock.transform.parent = module.transform.parent;
                        }

                        if (currentChild.gameObject.name.Contains("(Clone)"))
                        {
                            currentChild.gameObject.transform.parent = newBlock.transform;
                        }


                    }


                }

                if (my_Modules[i])
                    DestroyImmediate(my_Modules[i].gameObject);

            }



            GameObject[] myModules = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == "_block").ToArray();


            tt = myModules.Length;



            for (int i = 0; i < tt; i++)
            {

                float f = i / tt;

                EditorUtility.DisplayProgressBar("Combining meshes", "Please wait", f);

                module = myModules[i];

                GameObject newObjects = new GameObject("Combined meshes");
                newObjects.transform.parent = module.transform.parent;
                newObjects.transform.localPosition = Vector3.zero;
                newObjects.transform.localRotation = Quaternion.identity;

                CombineMeshes(module.gameObject, newObjects);

            }

            EditorUtility.ClearProgressBar();


        }

        generateLightmapUVs = GUILayout.Toggle(generateLightmapUVs, "Generate Lightmap UVs", GUILayout.Width(240));

        GUILayout.EndVertical();

    }


    private TrafficSystem trafficSystem;

    private void AddVehicles(int right_Hand = 0)
    {

        trafficSystem = FindObjectOfType<TrafficSystem>();

        if (!trafficSystem)
        {
            Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Fantastic City Generator/Traffic System/Traffic System.prefab", (typeof(GameObject))));
            trafficSystem = FindObjectOfType<TrafficSystem>();

        }

        if (!trafficSystem)
        {
            Debug.LogError("Add the Traffic System.prefab to Hierarchy");
            return;
        }
        else trafficSystem.name = "Traffic System";

        if (trafficSystem)
        {
            DestroyImmediate(GameObject.Find("CarContainer"));
            trafficSystem.LoadCars(right_Hand);
        }
    }

    private void InverseCarDirection(int trafficHand)
    {

        if (FindObjectOfType<TrafficSystem>())
            trafficSystem = FindObjectOfType<TrafficSystem>();

        if (!trafficSystem)
        {
            //Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Fantastic City Generator/Traffic System/Traffic System.prefab", (typeof(GameObject))));
            trafficSystem = AssetDatabase.LoadAssetAtPath("Assets/Fantastic City Generator/Traffic System/Traffic System.prefab", (typeof(TrafficSystem))) as TrafficSystem;
        }

        if (!trafficSystem)
        {
            Debug.LogError("Not Found System.prefab");
            return;
        }

        trafficSystem.DeffineDirection(trafficHand);

        if (GameObject.Find("CarContainer"))
            AddVehicles((trafficLightHand == 1 && japanTrafficLight) ? 2 : trafficLightHand);

    }

    private List<GameObject> newObjects = new List<GameObject>();


    public void CombineMeshes(GameObject objs, GameObject _Objects)
    {



        // Preserve Cloths
        Component[] temp = objs.GetComponentsInChildren(typeof(Cloth));
        foreach (Cloth currentChild in temp)
        {
            currentChild.gameObject.transform.parent = _Objects.transform;
            //currentChild.gameObject.isStatic = false;
        }


        //Preserve BoxCollider components
        temp = objs.GetComponentsInChildren(typeof(BoxCollider));
        foreach (BoxCollider currentChild in temp)
        {

            GameObject bc = new GameObject("BoxCollider");
            bc.transform.position = currentChild.transform.position;
            bc.transform.rotation = currentChild.transform.rotation;
            bc.transform.localScale = currentChild.transform.localScale;
            bc.transform.parent = _Objects.transform;

            UnityEditorInternal.ComponentUtility.CopyComponent(currentChild);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bc);

        }

        //Preserve MeshCollider components
        temp = objs.GetComponentsInChildren(typeof(MeshCollider));
        foreach (MeshCollider currentChild in temp)
        {

            GameObject bc = new GameObject("MeshCollider");
            bc.transform.position = currentChild.transform.position;
            bc.transform.rotation = currentChild.transform.rotation;
            bc.transform.localScale = currentChild.transform.parent.localScale;

            bc.transform.parent = _Objects.transform;

            UnityEditorInternal.ComponentUtility.CopyComponent(currentChild);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bc);

        }



        newObjects.Clear();

        Combine2(objs, _Objects);

    }




    private void Combine2(GameObject _objs, GameObject _Objects)
    {



        GameObject oldGameObjects = _objs;

        Component[] filters = GetMeshFilters(_objs);

        Matrix4x4 myTransform = _objs.transform.worldToLocalMatrix;
        Hashtable materialToMesh = new Hashtable();

        for (int i = 0; i < filters.Length; i++)
        {


            MeshFilter filter = (MeshFilter)filters[i];
            Renderer curRenderer = filters[i].GetComponent<Renderer>();
            Mesh_CombineUtility.MeshInstance instance = new Mesh_CombineUtility.MeshInstance();
            instance.mesh = filter.sharedMesh;
            if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
            {
                instance.transform = myTransform * filter.transform.localToWorldMatrix;

                Material[] materials = curRenderer.sharedMaterials;
                for (int m = 0; m < materials.Length; m++)
                {


                    instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                    try
                    {
                        ArrayList objects = (ArrayList)materialToMesh[materials[m]];

                        if (objects != null)
                            objects.Add(instance);
                        else
                        {
                            objects = new ArrayList();
                            objects.Add(instance);
                            materialToMesh.Add(materials[m], objects);
                        }


                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + "   Verify materials in " + curRenderer.name);

                    }



                }
            }
        }



        foreach (DictionaryEntry mtm in materialToMesh)
        {
            ArrayList elements = (ArrayList)mtm.Value;

            Mesh_CombineUtility.MeshInstance[] instances = (Mesh_CombineUtility.MeshInstance[])elements.ToArray(typeof(Mesh_CombineUtility.MeshInstance));


            Material mat = (Material)mtm.Key;

            GameObject go = new GameObject(mat.name);

            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.position = Vector3.zero;

            go.AddComponent(typeof(MeshFilter));
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = (Material)mtm.Key;


            MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
            filter.sharedMesh = Mesh_CombineUtility.Combine(instances, false);

            newObjects.Add(go);

        }

        if (newObjects.Count < 1)
        {
            return;
        }


        DestroyImmediate(oldGameObjects);


        if (newObjects.Count > 0)
        {
            for (int x = 0; x < newObjects.Count; x++)
            {


                newObjects[x].transform.parent = _Objects.transform;
                newObjects[x].transform.localPosition = Vector3.zero;
                newObjects[x].transform.localRotation = Quaternion.identity;

                // Generate Lightmap UVs ?
                if (generateLightmapUVs)
                {
                    Unwrapping.GenerateSecondaryUVSet(newObjects[x].GetComponent<MeshFilter>().sharedMesh);
                }



            }
        }





    }

    private Component[] GetMeshFilters(GameObject objs)
    {
        List<Component> filters = new List<Component>();
        Component[] temp = null;

        temp = objs.GetComponentsInChildren(typeof(MeshFilter));
        for (int y = 0; y < temp.Length; y++)
            filters.Add(temp[y]);

        return filters.ToArray();

    }



    public static List<T> LoadAllPrefabsOfType<T>(string path) where T : MonoBehaviour
    {
        if (path != "")
        {
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
        }

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] fileInf = dirInfo.GetFiles("*.prefab");

        //loop through directory loading the game object and checking if it has the component you want
        List<T> prefabComponents = new List<T>();
        foreach (FileInfo fileInfo in fileInf)
        {
            string fullPath = fileInfo.FullName.Replace(@"\", "/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

            if (prefab != null)
            {
                T hasT = prefab.GetComponent<T>();
                if (hasT != null)
                {
                    prefabComponents.Add(hasT);
                }
            }
        }
        return prefabComponents;
    }







}