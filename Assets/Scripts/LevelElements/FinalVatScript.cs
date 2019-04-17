using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalVatScript : MonoBehaviour
{

    public GameObject boss;
    public bool hit = false;
    public AudioSource audio;
    public AudioClip hitsound;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HealthStats>().OnDeath = OnDeath;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDeath(float overkill)
    {
        GetComponent<Vat>().OnDeath(overkill);
        boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
    }

    public void destroyed()
    {
        boss.GetComponent<BossEnemy>().StartCoroutine(boss.GetComponent<BossEnemy>().FadeOutAndExit());
    }

}
