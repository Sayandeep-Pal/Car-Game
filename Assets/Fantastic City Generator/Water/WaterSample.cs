using System;
using UnityEngine;

    public class WaterSample : MonoBehaviour
    {

        Renderer r;
        Material mat;

        void Start()
        {
            r  = GetComponent<Renderer>();
            if(r)
            mat = r.sharedMaterial;
        }

        void Update()
        {
            
            if (!r)
                return;

            if (!mat)
                return;

            Vector4 waveSpeed = mat.GetVector("WaveSpeed");
            float waveScale = mat.GetFloat("_WaveScale");
            float t = Time.time / 20.0f;

            Vector4 offset4 = waveSpeed * (t * waveScale);
            Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x, 1.0f), Mathf.Repeat(offset4.y, 1.0f),
                Mathf.Repeat(offset4.z, 1.0f), Mathf.Repeat(offset4.w, 1.0f));
            mat.SetVector("_WaveOffset", offsetClamped);

        }
    }
