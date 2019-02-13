using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killafteraudio : MonoBehaviour{

    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audio.isPlaying == false)
    {
        Destroy(gameObject);
    }
    }
}
