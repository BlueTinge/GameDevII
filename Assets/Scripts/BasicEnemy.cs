using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
public class BasicEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float hopDistance;
    [SerializeField] private float hopTime;
    [SerializeField] private float hopDelay;
    [SerializeField] private float coolDown;
    [SerializeField] private float lungeDistance;
    [SerializeField] private float lungeDelay;
    [SerializeField] private float lungeTime;
    [SerializeField] private float lungeCooldown;
    [SerializeField] private float visionRadius;
    [SerializeField] private float attackRadius;
    
    private Transform target;
    private BehaviorTree behaviorTree;
    private float jumpSpeed;
    private float horizontalJumpSpeed;

    private float lungeVSpeed;
    private float lungeHSpeed;
    private Rigidbody rb;
    private bool shouldJump;
    private bool shouldLunge;
    private Vector3 targetPos;

    void Awake()
    {
        horizontalJumpSpeed = hopDistance / hopTime;
        float hopHeight = 0.5f * Physics.gravity.y * hopTime * hopTime / 4;
        jumpSpeed = Mathf.Sqrt(2 * Physics.gravity.y * hopHeight);

        lungeHSpeed = lungeDistance / lungeTime;
        float lungeHeight = 0.5f * -Physics.gravity.y * lungeTime * lungeTime;
        lungeVSpeed = Mathf.Sqrt(2 * -Physics.gravity.y * lungeHeight);
        shouldLunge = false;
        shouldJump = false;
    }
    void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        behaviorTree = new BehaviorTree
        (
            new SelectorTask(new ITreeTask[]
            {
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, target, attackRadius),
                    new WhileTask
                    (
                        new NotTask
                        (
                            new DelayTask(lungeDelay)
                        ),
                        new BasicTarget(this)
                    ),
                    new BasicAttack(this, lungeTime),
                    new DelayTask(lungeCooldown),
                }),
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, target, visionRadius),
                    new WhileTask
                    (
                        new NotTask
                        (
                            new DelayTask(hopDelay)
                        ),
                        new BasicTarget(this)
                    ),
                    new BasicMove(this, hopTime),
                    new DelayTask(coolDown),
                })
            })
        );
    }

    void Update()
    {
        behaviorTree.Update();
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        if(shouldLunge)
        {
            velocity = targetPos - transform.position;
            velocity.y = 0;
            velocity = velocity.normalized;
            velocity *= lungeHSpeed;
            velocity.y = lungeVSpeed;
            rb.velocity = velocity;
        }
        else if(shouldJump)
        {
            velocity = targetPos - transform.position;
            velocity.y = 0;
            velocity = velocity.normalized;
            velocity *= horizontalJumpSpeed;
            velocity.y = jumpSpeed;
            rb.velocity = velocity;
        }

        shouldLunge = false;
        shouldJump = false;
    }

    void OnCollisionEnter(Collision c)
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    public void Target()
    {
        targetPos = target.position;
    }

    public void Attack()
    {
        shouldLunge = true;
    }

    public void Move()
    {
        shouldJump = true;
    }
}
