using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossthemescript : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip intro;
    public AudioClip looper;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(audio.isPlaying == false)
        {
            audio.clip = looper;
            audio.loop = looper;
            audio.Play();
        }
    }
}
