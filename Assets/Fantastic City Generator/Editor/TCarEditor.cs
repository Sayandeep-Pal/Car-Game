using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FCG
{

    [CustomEditor(typeof(TrafficCar))]
    public class TCarEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TrafficCar TF = (TrafficCar)target;

            if (GUILayout.Button("Generate WheelColliders"))
            {
                if (TF.gameObject.activeInHierarchy)
                    TF.Configure();
                else
                    Debug.LogWarning("Place the object in the hierarchy");
            }

        }


    }
}
