using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossthemescript : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip intro;
    public AudioClip looper;
    public bool bossdead = false;
    public float rateofdecrease = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(audio.isPlaying == false && bossdead == false)
        {
            audio.clip = looper;
            audio.loop = looper;
            audio.Play();
        }
    }

    public void fadeoutvoid()
    {
        bossdead = true;
        StartCoroutine(fadeout());
    }

    IEnumerator fadeout()
    {
        while(audio.volume > 0)
        {
            audio.volume -= rateofdecrease;
            yield return new WaitForSeconds(0.2f);
        }

        audio.Stop();

    }
}
