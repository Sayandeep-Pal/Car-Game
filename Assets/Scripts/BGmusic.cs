using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private AudioSource Song;
    public AudioClip BGsong;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BGsong != null) 
        {
            Song.PlayOneShot(BGsong);
        }
    }
}
