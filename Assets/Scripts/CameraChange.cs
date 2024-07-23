using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledOrDisabled : MonoBehaviour
{
    public GameObject Camera;

    public void Trigger()
    {
        if (Camera.activeInHierarchy == false)
        {
            Camera.SetActive(true);

        }
        else
        {
            Camera.SetActive(false);
        }
    }
}