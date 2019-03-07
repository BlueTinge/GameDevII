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
    private Transform target;

    void Awake()
    {
        healthStats = GetComponent<HealthStats>();
        laserCharge = GetComponentInChildren<EyeCharge>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        behaviorTree = new BehaviorTree
        (
            new SequenceTask(new ITreeTask[]
            {
                new BasicAttack(this, windupTime),
                new DelayTask(coolDown)
            })
        );
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
 
        behaviorTree.Update();
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