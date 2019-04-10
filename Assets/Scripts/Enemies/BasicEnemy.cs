using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class BasicEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]private float maxAccel;
    [SerializeField]private float maxSpeed;
    [SerializeField]private float targetRadius;
    [SerializeField]private float accelTime;
    [SerializeField] private float hopTime;
    [SerializeField] private float hopDelay;
    [SerializeField] private float pursueEstimate;
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
    [SerializeField] private float KnockbackFactor;
    [SerializeField] private AnimationCurve deathFade;
    [SerializeField] private GameObject hurtBox;
    
    private Transform target;
    private Rigidbody targetRb;
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
    public GameObject hurtingsfx;
    public bool hurting = false;
    public AudioClip diessfx;

    public GameObject DeathParticlePrefab;

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
        targetRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        target = targetRb.transform;
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
            StandardShaderUtils.ChangeRenderMode(renderer.materials[i], StandardShaderUtils.BlendMode.Opaque);
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
            
            rb.AddTorque((impulse - rb.angularVelocity).magnitude * Mathf.Sign(impulse.y) * Vector3.up, ForceMode.VelocityChange);
            rb.angularVelocity = rb.angularVelocity.y * Vector3.up;
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        if(shouldJump)
        {
            Vector3 accel = CalcAccel();

            rb.AddForce(accel, ForceMode.Acceleration);
        }
        else
        {
            Stop();
        }
    }

    public bool Target()
    {
        targetPos = target.position;
        targetPos.y = transform.position.y;
        targetPos += targetRb.velocity * pursueEstimate /
            maxSpeed * (targetPos - transform.position).magnitude;

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
        audio.volume = 0.7f;
        audio.Play();
        animator.SetBool("windup", false);
        animator.SetBool("attacking", true);
        Vector3 knockback = (targetPos - transform.position).normalized * KnockbackFactor;
        knockback.y = 0;
        hurtBox.AddComponent<Attack>().Initialize(5, knockback,
            lungeTime, gameObject);
    }

    public void Move()
    {
        audio.clip = walkingsfx;
        audio.volume = 0.7f;
        audio.Play();
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
        audio.volume = 1f;
        audio.Play();
        animator.SetBool("windup", true);
        animator.SetBool("attacking", false);
    }

    private void Die()
    {
        audio.clip = diessfx;
        audio.volume = 1f;
        audio.Play();
        animator.SetBool("dead", true);
        dead = true;
        deathTime = Time.time;
        foreach(Material m in renderer.materials)
        {
            StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Fade);
        }
        Instantiate(DeathParticlePrefab,transform.position, Quaternion.identity);
    }

    private Vector3 CalcAccel()
    {
        Vector3 t = targetPos;
        t.y = transform.position.y;

        Vector3 v = t - transform.position;
        float dist = v.magnitude;
        if(dist < targetRadius)
        {
            Stop();
            return Vector3.zero;
        }

        float speed = maxSpeed;

        Vector3 a = v.normalized * speed - rb.velocity;
        a /= accelTime;
        if(a.sqrMagnitude > maxAccel * maxAccel)
        {
            return a.normalized * maxAccel;
        }
        else
        {
            return a;
        }
    }

    private void Stop()
    {
        if(rb.velocity.sqrMagnitude <= accelTime * accelTime * maxAccel * maxAccel)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.AddForce(-maxAccel * rb.velocity.normalized, ForceMode.Acceleration);
        }
    }

    private IEnumerator TakeDamage()
    {
        if(hurting == false)
        {
            Instantiate(hurtingsfx);
            hurting = true;
        }
        HealthBar.fillAmount = healthStats.CurrentHealth / healthStats.MaxHealth;
        foreach (var v in renderer.materials)
        {
            v.color = Color.red;
        }
        yield return new WaitForSeconds(healthStats.GetImmunity());
        for (int i = 0; i < colors.Length; ++i)
        {
            renderer.materials[i].color = colors[i];
        }
        hurting = false;
    }
}
