using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossthemescript : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip intro;
    public AudioClip looper;
    public float rateofdecrease = 0.1f;

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

    void fadeout()
    {
        if(audio.volume > 0)
        {
            audio.volume -= rateofdecrease;
        }
        else
        {
            audio.volume = 0;
        }
    }
}
