using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public AudioSource Sound;
    public AudioClip Clip;
    private BoxCollider CarBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider != null)
        {
            Sound.PlayOneShot(Clip);
        }
    }
}
