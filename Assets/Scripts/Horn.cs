using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hornSound : MonoBehaviour
{
    private AudioSource Sound;
    public AudioClip Horn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Sound != null && Horn != null)
        {
            Sound.PlayOneShot(Horn);
        }
        
    }
}
