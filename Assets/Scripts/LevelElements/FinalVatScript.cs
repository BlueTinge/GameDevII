using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalVatScript : MonoBehaviour
{

    public GameObject boss;
    public bool hit = false;
    public bool readytogo = false;
    public AudioSource audio;
    public AudioClip endsong;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HealthStats>().OnDeath = OnDeath;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(readytogo == true)
        {
            if (audio.isPlaying == false)
            {
                boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
            }
        }
    }

    public void OnDeath(float overkill)
    {
        GetComponent<Vat>().OnDeath(overkill);
        StartCoroutine(onefinalsong());
        //boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
    }

    public IEnumerator onefinalsong()
    {
        yield return new WaitForSeconds(1.5f);
        audio.clip = endsong;
        audio.Play();
        readytogo = true;
    }

    public void destroyed()
    {
        boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
    }

}
