using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class EyeEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]private float moveTime;
    [SerializeField]private float moveSpeed;
    [SerializeField]private float windupTime;
    [SerializeField]private float coolDown;
    private BehaviorTree behaviorTree;
    private EyeCharge laserCharge;
    private HealthStats healthStats;
    private Rigidbody rb;

    void Start()
    {
        healthStats = GetComponent<HealthStats>();
        laserCharge = GetComponentInChildren<EyeCharge>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    public bool Target()
    {
        return true;
    }
    public void Move()
    {

    }

    public void Attack()
    {
        if(!laserCharge.IsTriggered())
        {
            laserCharge.SetChargeTime(windupTime);
            laserCharge.Trigger();
        }
    }
}