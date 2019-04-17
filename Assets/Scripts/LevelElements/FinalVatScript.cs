using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalVatScript : MonoBehaviour
{

    public GameObject boss;
    public bool hit = false;
    public bool readytogo = false;
    public bool nomoresongs = false;
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
                print("OH");
                boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
            }
        }
    }

    public void OnDeath(float overkill)
    {
        if (nomoresongs == false)
        {
            GetComponent<Vat>().OnDeath(overkill);
            StartCoroutine(onefinalsong());
        }
    }

    public IEnumerator onefinalsong()
    {
        if (nomoresongs == false)
        {
            nomoresongs = true;
            yield return new WaitForSeconds(0.1f);
            GetComponent<Vat>().shouldplay = false;
            yield return new WaitForSeconds(1.4f);
            print("PLAYFINALVAT");
            audio.clip = endsong;
            audio.Play();
            readytogo = true;
        }
    }

    public void destroyed()
    {
        boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
    }

}
