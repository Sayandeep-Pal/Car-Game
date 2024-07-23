
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICON.Utilities;

namespace FCG
{
    [CustomEditor(typeof(FCGWaypointsContainer))]

    public class FCGWPEditor : Editor
    {

        FCGWaypointsContainer wpScript;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            wpScript = (FCGWaypointsContainer)target;

            if (GUI.changed)
            {
                wpScript.RefreshAllWayPoints();
            }

        }


        void OnSceneGUI()
        {

            Event e = Event.current;
            wpScript = (FCGWaypointsContainer)target;

            if (e != null)
            {


                if (e.isMouse && e.shift && e.type == EventType.MouseDown)
                {

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, 5000.0f))
                    {

                        GetWaypoints();

                        Vector3 newTilePosition = hit.point;

                        GameObject wp = new GameObject(wpScript.name + " - " + (wpScript.waypoints.Count + 1).ToString("00"));

                        wp.transform.position = newTilePosition + Vector3.up * 0.1f;
                        wp.transform.SetParent(wpScript.transform);

                        GetWaypoints();

                        wpScript.RefreshAllWayPoints();



                    }

                    Selection.activeObject = wpScript.gameObject;



                }
                else if (e.isMouse && !e.shift && e.type == EventType.MouseDown)
                {


                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, 5000.0f))
                    {

                        Vector3 newTilePosition = hit.point;

                        FCGWaypointsContainer[] tArray = GameObject.FindObjectsOfType<FCGWaypointsContainer>();

                        for (int f = 0; f < tArray.Length; f++)
                        {
                            if (tArray[f].transform.childCount > 1)
                            {

                                Transform[] points = tArray[f].transform.GetComponentsInChildren<Transform>();

                                for (int i = 0; i < points.Length; i++)
                                    if (Vector3.Distance(points[i].position, newTilePosition) < 1f)
                                    {
                                        Selection.activeObject = points[i];
                                        wpScript.RefreshAllWayPoints();
                                        return;
                                    }

                            }
                        }


                    }


                }


                if (wpScript)
                    Selection.activeGameObject = wpScript.gameObject;


            }

        }



        public void GetWaypoints()
        {


            wpScript.waypoints = new List<Transform>();

            Transform[] allTransforms = wpScript.transform.GetComponentsInChildren<Transform>();

            for (int i = 1; i < allTransforms.Length; i++)
            {


                allTransforms[i].name = wpScript.name + " - " + i.ToString("00");

                wpScript.waypoints.Add(allTransforms[i]);
                allTransforms[i].gameObject.SetIcon(LabelIcon.Yellow);

            }


            wpScript.WaypointsSetAngle();



        }



    }
}