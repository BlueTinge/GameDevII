using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class BasicEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float hopDistance;
    [SerializeField] private float hopTime;
    [SerializeField] private float hopDelay;
    [SerializeField] private float dragForce;
    [SerializeField] private float coolDown;
    [SerializeField] private float lungeDelay;
    [SerializeField] private float targetTimeout;
    [SerializeField] private float lungeTime;
    [SerializeField] private float lungeCooldown;
    [SerializeField] private float fadeoutDelay;
    [SerializeField] private float fadeoutLength;
    [SerializeField] private float visionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float seekAngle;
    [SerializeField] private AnimationCurve deathFade;
    [SerializeField] private GameObject hurtBox;
    
    private Transform target;
    private BehaviorTree behaviorTree;
    private float targetTime;
    private bool targeting;
    private Rigidbody rb;
    private bool shouldJump;
    private bool shouldTurn;
    private Vector3 targetPos;
    private HealthStats healthStats;
    private SkinnedMeshRenderer renderer;
    private Color[] colors;
    private Animator animator;
    private bool dead;
    private float deathTime;

    public Image HealthBar;

    AudioSource audio;
    public AudioClip walkingsfx;
    public AudioClip windupsfx;
    public AudioClip attacksfx;
    public AudioClip hurtingsfx;
    public AudioClip diessfx;

    void Awake()
    {
        shouldJump = false;
        shouldTurn = false;
        targeting = false;
        dead = false;
    }
    void Start()
    {
        audio = GetComponent<AudioSource>();
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        healthStats = GetComponent<HealthStats>();
        healthStats.OnDeath = (overkill) => {Die();};
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
                        new NotTask
                        (
                            new DelayTask(lungeDelay)
                        ),
                        new BasicTarget(this)
                    ),
                    new BasicAttack(this, lungeTime),
                    new CallTask(()=>{animator.SetBool("attacking", false); return true;}),
                    new DelayTask(lungeCooldown)
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
                    new CallTask(()=>{StopMove(); return true;}),
                    new DelayTask(coolDown)
                })
            })
        );
    }

    void Update()
    {
        if(!dead)
        {
            behaviorTree.Update();
        }
        else
        {
            if(Time.time - deathTime > fadeoutDelay + fadeoutLength)
            {
                Destroy(gameObject);
            }
            else if(Time.time - deathTime > fadeoutDelay)
            {
                float opacity = deathFade.Evaluate((Time.time - (deathTime + fadeoutDelay))/fadeoutLength);
                foreach(Material m in renderer.materials)
                {
                    m.color = new Color(m.color.g, m.color.g, m.color.b, opacity);
                }
            }
        }
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

        if(shouldJump)
        {
            Vector3 force = targetPos - transform.position;
            force.y = 0;
            force = force.normalized;
            force *= dragForce;
            force += Vector3.up * (-Physics.gravity.y);
            Debug.DrawRay(transform.position, force, Color.yellow);

            rb.AddForce(force, ForceMode.Acceleration);
        }
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
        audio.clip = attacksfx;
        audio.Play();
        animator.SetBool("windup", false);
        animator.SetBool("attacking", true);
        hurtBox.AddComponent<Attack>().Initialize(5, (targetPos - transform.position).normalized,
            lungeTime, gameObject);
    }

    public void Move()
    {
        shouldJump = true;
        animator.SetBool("moving", true);
    }
    private void StopMove()
    {
        animator.SetBool("moving", false);
        shouldJump = false;
    }

    private void Windup()
    {
        audio.clip = windupsfx;
        audio.Play();
        animator.SetBool("windup", true);
        animator.SetBool("attacking", false);
    }

    private void Die()
    {
        animator.SetBool("dead", true);
        dead = true;
        deathTime = Time.time;
    }

    private IEnumerator TakeDamage()
    {
        HealthBar.fillAmount = healthStats.CurrentHealth / healthStats.MaxHealth;
        foreach (var v in renderer.materials)
        {
            v.color = Color.red;
        }
        yield return new WaitForSeconds(healthStats.GetImmunity());
        for(int i = 0; i < colors.Length; ++i)
        {
            renderer.materials[i].color = colors[i];
        }

    }
}
