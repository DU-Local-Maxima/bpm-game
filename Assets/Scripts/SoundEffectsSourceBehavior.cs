using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsSourceBehavior : MonoBehaviour
{
    public AudioClip collisionFx;

    private AudioSource myAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCollisionFx()
    {
        if (!myAudioSource.isPlaying)
        {
            myAudioSource.clip = collisionFx;
            myAudioSource.Play();
        }
    }
}
