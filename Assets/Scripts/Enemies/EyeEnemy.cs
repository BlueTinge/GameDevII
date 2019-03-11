using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class EyeEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]private float moveTime;
    [SerializeField]private float maxAccel;
    [SerializeField]private float maxSpeed;
    [SerializeField]private float slowRadius;
    [SerializeField]private float targetRadius;
    [SerializeField]private float accelTime;
    [SerializeField]private float goalDistance;
    [SerializeField]private float windupTime;
    [SerializeField]private float coolDown;
    private BehaviorTree behaviorTree;
    private EyeCharge laserCharge;
    private HealthStats healthStats;
    private Rigidbody rb;
    private Transform target;
    private Vector3 accel;
    private bool canMove;

    void Awake()
    {
        healthStats = GetComponent<HealthStats>();
        laserCharge = GetComponentInChildren<EyeCharge>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        accel = Vector3.zero;
        canMove = true;
        behaviorTree = new BehaviorTree
        (
            new SequenceTask(new ITreeTask[]
            {
                new WhileTask
                (
                    new NotTask
                    (
                        new DelayTask(moveTime)
                    ),
                    new BasicMove(this, 0)
                ),
                new CallTask(() => {EndMove(); return true;}),
                new BasicAttack(this, windupTime),
                new DelayTask(coolDown)
            })
        );
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        if(!canMove) Stop();
        rb.AddForce(accel, ForceMode.Acceleration);
    }

    public bool Target()
    {
        return true;
    }
    public void Move()
    {
        canMove = true;
        Vector3 dir = transform.position - target.position;
        dir.y = 0;
        dir = dir.normalized * goalDistance;
        Vector3 t = dir + target.position;
        t.y = transform.position.y;

        Vector3 v = t - transform.position;
        float dist = v.magnitude;
        if(dist < targetRadius)
        {
            accel = Vector3.zero;
            return;
        }

        float speed = maxSpeed;
        if(dist < slowRadius)
        {
            speed = maxSpeed * dist / slowRadius;
        }

        Vector3 a = v - rb.velocity;
        a /= accelTime;
        if(a.sqrMagnitude > maxAccel * maxAccel)
        {
            accel = a.normalized * maxAccel;
        }
        else
        {
            accel = a;
        }
    }

    private void Stop()
    {
        if(rb.velocity.sqrMagnitude <= accelTime * accelTime * maxAccel * maxAccel)
        {
            accel = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        else
        {
            accel = -maxAccel * rb.velocity.normalized;
        }
    }
    public void EndMove()
    {
        canMove = false;
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