using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCharge : MonoBehaviour
{
    [SerializeField]private GameObject projectile;
    [SerializeField]private float projectectileSpeed;
    [SerializeField]private float spawnOffset;
    private float chargeTime;
    private bool isTriggered;
    private bool isComplete;
    private float triggerTime;
    private Vector3 initalSize;
    
    void Awake()
    {
        chargeTime = 0;
        isTriggered = false;
        isComplete = false;
        triggerTime = Time.time;
        initalSize = transform.localScale;
    }

    void Start()
    {
        isTriggered = false;
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if(!isTriggered) return;
        float scale = (Time.time - triggerTime)/chargeTime;
        transform.localScale = initalSize * Mathf.Clamp01(scale);
        if(Time.time - triggerTime >= chargeTime)
        {
            isComplete = true;
        }
    }

    public void SetChargeTime(float chargeTime)
    {
        this.chargeTime = chargeTime;
    }

    public void Trigger()
    {
        isComplete = false;
        isTriggered = true;
        gameObject.SetActive(true);
        triggerTime = Time.time;
    }

    public bool IsTriggered()
    {
        return isTriggered;
    }

    public bool IsComplete()
    {
        return isComplete;
    }

    void Fire()
    {
        GameObject g = (GameObject)Instantiate(projectile, transform.position + transform.forward * spawnOffset, transform.rotation);
        g.GetComponent<Projectile>()?.SetSpeed(projectectileSpeed);
        isTriggered = false;
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }
}
