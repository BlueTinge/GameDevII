using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthStats))]
public class Balloon : MonoBehaviour
{
    HealthStats MyHealthStats;

    // Start is called before the first frame update
    void Start()
    {
        MyHealthStats = GetComponent<HealthStats>();
        MyHealthStats.OnDeath = delegate (float damage)
        {
            GetComponent<Renderer>().material.color = Color.green;
            Destroy(gameObject, 1f);
        };
        MyHealthStats.OnDamage = delegate (float damage)
        {
            if(MyHealthStats.CurrentHealth > 0) GetComponent<Renderer>().material.color = Color.yellow;
        };
        MyHealthStats.OnImmunityEnd = delegate ()
        {
            GetComponent<Renderer>().material.color = Color.red;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
