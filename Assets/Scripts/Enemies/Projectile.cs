using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]private float killTime;
    [SerializeField]private float damage;
    
    Rigidbody rb;
    private float startTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GetComponent<Attack>().Initialize(damage, Vector3.zero, 0, gameObject);
        startTime = Time.time;
    }

    void Update()
    {
        if(Time.time - startTime > killTime)
        {
            Destroy(gameObject);
        }
    }
    public void SetSpeed(float speed)
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision c)
    {
        StartCoroutine(Kill());
    }

    private IEnumerator Kill()
    {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
