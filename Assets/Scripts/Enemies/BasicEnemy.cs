using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class BasicEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float hopDistance;
    [SerializeField] private float hopTime;
    [SerializeField] private float hopDelay;
    [SerializeField] private float coolDown;
    [SerializeField] private float lungeDistance;
    [SerializeField] private float lungeDelay;
    [SerializeField] private float targetTimeout;
    [SerializeField] private float lungeTime;
    [SerializeField] private float lungeCooldown;
    [SerializeField] private float visionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float seekAngle;
    
    private Transform target;
    private BehaviorTree behaviorTree;
    private float jumpSpeed;
    private float horizontalJumpSpeed;
    private float lungeVSpeed;
    private float lungeHSpeed;
    private float targetTime;
    private bool targeting;
    private Rigidbody rb;
    private bool shouldJump;
    private bool shouldLunge;
    private bool shouldTurn;
    private Vector3 targetPos;
    private HealthStats healthStats;
    private SkinnedMeshRenderer renderer;
    private Color[] colors;
    private Animator animator;

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
        shouldTurn = false;
        targeting = false;
    }
    void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        healthStats = GetComponent<HealthStats>();
        healthStats.OnDeath = (overkill) => {Destroy(gameObject);};
        healthStats.OnDamage = (damage) => {StartCoroutine(TakeDamage());};
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        colors = new Color[renderer.materials.Length];
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("attacking", false);
        for(int i = 0; i < colors.Length; ++i)
        {
            colors[i] = renderer.materials[i].color;
        }
        behaviorTree = new BehaviorTree
        (
            new SelectorTask(new ITreeTask[]
            {
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, target, attackRadius),
                    new CallTask(() => {Windup(); return true;}),
                    new WhileTask
                    (
                        new SequenceTask(new ITreeTask[]{
                            new NotTask
                            (
                                new DelayTask(lungeDelay)
                            ),
                            new CallTask(() => {animator.speed = 0; return true;})
                        }),
                        new BasicTarget(this)
                    ),
                    new CallTask(() => {animator.speed = 1; return true;}),
                    new BasicAttack(this, lungeTime),
                    new CallTask(()=>{animator.SetBool("attacking", false); return true;}),
                    new DelayTask(lungeCooldown),
                }),
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, target, visionRadius),
                    new WhileTask
                    (
                        new SequenceTask(new ITreeTask[]{
                            new NotTask
                            (
                                new DelayTask(hopDelay)
                            ),
                            new CallTask(() => {animator.speed = 0; return true;})
                        }),
                        new BasicTarget(this)
                    ),
                    new CallTask(() => {animator.speed = 1; return true;}),
                    new BasicMove(this, hopTime),
                    new CallTask(()=>{animator.SetBool("moving", false); return true;}),
                    new DelayTask(coolDown)
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
        if(transform.up != Vector3.up)
        {
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z)));
        }

        Vector3 forward = transform.forward;
        Vector3 faceTarget = targetPos - transform.position;
        forward.y = 0;
        faceTarget.y = 0;

        float angle = Vector3.SignedAngle(forward, faceTarget, Vector3.up);

        if(shouldTurn)
        {
            Vector3 impulse = rotateSpeed * Mathf.Deg2Rad * Vector3.up;
            shouldTurn = false;
            if(Mathf.Abs(angle) < seekAngle)
            {
                impulse *= angle / seekAngle;
            }
            else if(angle < 0)
            {
                impulse *= -1;
            }
            rb.AddTorque(impulse - rb.angularVelocity, ForceMode.VelocityChange);
            rb.angularVelocity = new Vector3(0, rb.angularVelocity.y, 0);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 velocity = rb.velocity;
        if(shouldLunge)
        {
            velocity = transform.forward;
            velocity.y = 0;
            velocity = velocity.normalized;
            velocity *= lungeHSpeed;
            velocity.y = lungeVSpeed;

            rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
        }
        else if(shouldJump)
        {
            velocity = targetPos - transform.position;
            velocity.y = 0;
            velocity = velocity.normalized;
            velocity *= horizontalJumpSpeed;
            velocity.y = jumpSpeed;

            rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
        }

        shouldLunge = false;
        shouldJump = false;
    }

    void OnCollisionEnter(Collision c)
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    public bool Target()
    {
        targetPos = target.position;
        shouldTurn = true;
        Vector3 t = targetPos - transform.position;
        t.y = 0;
        Vector3 f = transform.forward;
        f.y = 0;
        if(Vector3.Angle(f, t) < seekAngle)
        {
            targeting = false;
            return true;
        }
        if(targeting)
        {
            if(Time.time - targetTime >= targetTimeout)
            {
                return true;
            }
        }
        else
        {
            targetTime = Time.time;
            targeting = true;
        }
        return false;
    }

    public void Attack()
    {
        shouldLunge = true;
        gameObject.AddComponent<Attack>().Initialize(5, (targetPos - transform.position).normalized,
            lungeTime, gameObject);
    }

    public void Move()
    {
        shouldJump = true;
        animator.SetBool("moving", true);
    }

    private IEnumerator TakeDamage()
    {
        
        foreach(var v in renderer.materials)
        {
            v.color = Color.red;
        }
        yield return new WaitForSeconds(healthStats.GetImmunity());
        for(int i = 0; i < colors.Length; ++i)
        {
            renderer.materials[i].color = colors[i];
        }
    }

    private void Windup()
    {
        animator.SetBool("attacking", true);
    }
}
