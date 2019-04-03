using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambienttoggler : MonoBehaviour
{
    int counter = 2;
    public AudioSource audio;
    public AudioClip ambient1intro;
    public AudioClip ambient1;
    public AudioClip ambient2;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = ambient1intro;
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (audio.isPlaying == false)
        {
            if (counter % 2 == 0)
            {
                audio.clip = ambient2;
            }
            if (counter % 2 == 1)
            {
                audio.clip = ambient1;
            }
            counter += 1;
            audio.Play();
        }
    }
}
