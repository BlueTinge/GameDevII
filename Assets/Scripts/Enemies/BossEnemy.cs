using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class DashTask : ITreeTask
{
    private BossEnemy be;
    private float trigerTime;
    private float duration;
    private Vector3 direction;

    public DashTask(BossEnemy boss, Vector3 relativeDirection, float dashTime)
    {
        be = boss;
        trigerTime = 0;
        direction = relativeDirection;
        duration = dashTime;
    }
    public TaskState state{get; private set;}
    
    public IEnumerable Update()
    {
        state = TaskState.continuing;
        trigerTime = Time.time;
        Vector3 rDir = be.transform.rotation * direction;
        be.Dash(rDir);
        do
        {
            yield return null;
        }
        while(Time.time - trigerTime < duration);
        be.StopDash();
        state = TaskState.success;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class HeavyAttack : ITreeTask
{
    public TaskState state{get; private set;}
    private BossEnemy self;
    private DelayTask delay;

    public HeavyAttack(BossEnemy self, float attackTime)
    {
        this.self = self;
        delay = new DelayTask(attackTime);
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        state = TaskState.continuing;
        self.HeavyAttack();
        foreach(object _ in delay.Update())
        {
            yield return null;
        }
        self.StopHeavy();
        state = TaskState.success;
    }

    public void Reset()
    {
        delay.Reset();
        state = TaskState.ready;
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class BossEnemy : MonoBehaviour, IEnemy
{

    [SerializeField] private AudioSource audio;
    [SerializeField] private GameObject musicplayer;
    [SerializeField] private GameObject gate;
    [SerializeField] private AudioClip step1;
    [SerializeField] private AudioClip step2;
    [SerializeField] private AudioClip step3;
    [SerializeField] private AudioClip step4;
    [SerializeField] private AudioClip step5;
    [SerializeField] private AudioClip step6;
    public List<AudioClip> steps = new List<AudioClip>();
    [SerializeField] private AudioClip takesdamage;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip lightswing;
    [SerializeField] private AudioClip dies;
    [SerializeField] private AudioClip heavyswing1;
    [SerializeField] private AudioClip heavyswing2;
    [SerializeField] private AudioClip heavyswingwindup;
    public List<AudioClip> heavyswings = new List<AudioClip>();
    [SerializeField] private int randomer;
    [SerializeField] private bool takingdamage = false;
    [SerializeField] private Image HealthBar;

    [SerializeField] private float healthCutoff;
    [SerializeField] private float[] normalWeights;
    [SerializeField] private float[] enrageWeights;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float lightWindup;
    [SerializeField] private float lightAttackTime;
    [SerializeField] private float lightCooldown;
    [SerializeField] private float heavyWindup;
    [SerializeField] private float heavyAttackTime;
    [SerializeField] private float heavyAttackDistance;
    [SerializeField] private float heavyCooldown;
    [SerializeField] private float lightDamage;
    [SerializeField] private float heavyDamage;
    [SerializeField] private float chargeArmor;
    [SerializeField] private float maxAccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float slowRadius;
    [SerializeField] private float targetRadius;
    [SerializeField] private float accelTime;
    [SerializeField] private float maxOmega;
    [SerializeField] private float maxAlpha;
    [SerializeField] private float slowDistance;
    [SerializeField] private float flickerSpeed;
    [SerializeField] private float flickerAlpha;
    [SerializeField] private GameObject attackObject;  

    private Transform player;
    private Rigidbody rb;
    private HealthStats healthStats;
    private BehaviorTree behaviorTree;
    private bool canTurn;
    private bool canMove;
    private bool canDash;
    private float alpha;
    private Vector3 accel;
    private Animator animator;
    private bool isAlive;
    private bool isFlickering;
    Material[] materials;
    private float normalArmor;

    void Start()
    {
        SetJointsActive(false);
        GetComponentInChildren<Weapon>().Holder = gameObject;
        GetComponentInChildren<Weapon>().SetCollidersEnabled(true);

        audio = GetComponent<AudioSource>();
        steps.Add(step1);
        steps.Add(step2);
        steps.Add(step3);
        steps.Add(step4);
        steps.Add(step5);
        steps.Add(step6);
        heavyswings.Add(heavyswing1);
        heavyswings.Add(heavyswing2);

        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        healthStats = GetComponent<HealthStats>();
        normalArmor = healthStats.Defense;
        healthStats.OnDeath = OnDeath;
        healthStats.OnDamage = OnDamage;
        healthStats.OnImmunityEnd = OnImmunityEnd;

        alpha = 0;
        canTurn = false;
        canMove = false;
        canDash = false;
        accel = Vector3.zero;
        isAlive = true;
        isFlickering = false;
        animator = GetComponentInChildren<Animator>();
        List<Material> mats = new List<Material>();
        foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mats.AddRange(mr.materials);
        }
        materials = mats.ToArray();

        behaviorTree = new BehaviorTree
        (
            new SelectorTask(new ITreeTask[]
            {
                new SequenceTask(new ITreeTask[]
                {
                    new CallTask(()=>{animator.SetBool("windupDone", false); return true;}),
                    new WhileTask
                    (
                        new NotTask
                        (
                            new BasicTarget(this)
                        ),
                        new BasicMove(this, 0)
                    ),
                    new CallTask(()=> healthStats.CurrentHealth <= healthCutoff),
                    new CloseTo(transform, player, targetRadius),
                    new RandomSelectTask(enrageWeights, new ITreeTask[]{
                        new SequenceTask(new ITreeTask[] {
                            new DashTask(this, Vector3.back, dashTime),
                            new CallTask(()=>{healthStats.Defense = chargeArmor; return true;}),
                            new CallTask(()=>{HeavyWindup(); return true;}),
                            new DelayTask(heavyWindup),
                            new HeavyAttack(this, heavyAttackTime),
                            new CallTask(()=>{healthStats.Defense = normalArmor; return true;}),
                            new DelayTask(heavyCooldown),
                            new CallTask(()=>{animator.SetBool("heavyAttack", false); return true;}),
                        }),
                        new SequenceTask(new ITreeTask[] {
                            new CallTask(()=>{LightWindup(); return true;}),
                            new DelayTask(lightWindup),
                            new BasicAttack(this, lightAttackTime),
                            new DelayTask(lightCooldown),
                            new CallTask(()=>{animator.SetBool("lightAttack", false); return true;}),
                        }),
                        new SequenceTask(new ITreeTask[] {
                            new DashTask(this, Vector3.right, dashTime),
                            new DelayTask(dashCooldown)
                        }),
                        new SequenceTask(new ITreeTask[] {
                            new DashTask(this, Vector3.left, dashTime),
                            new DelayTask(dashCooldown)
                        })
                    })
                }),
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, player, targetRadius),
                    new RandomSelectTask(normalWeights, new ITreeTask[]{
                        new SequenceTask(new ITreeTask[] {
                            new CallTask(()=>{LightWindup(); return true;}),
                            new DelayTask(lightWindup),
                            new BasicAttack(this, lightAttackTime),
                            new DelayTask(lightCooldown),
                            new CallTask(()=>{animator.SetBool("lightAttack", false); return true;}),
                        }),
                        new SequenceTask(new ITreeTask[] {
                            new DashTask(this, Vector3.right, dashTime),
                            new DelayTask(dashCooldown)
                        }),
                        new SequenceTask(new ITreeTask[] {
                            new DashTask(this, Vector3.left, dashTime),
                            new DelayTask(dashCooldown)
                        })
                    })
                })  
            })
        );
    }

    void Update()
    {
        if(!isAlive)
        {
            isFlickering = false;
            return;
        }

        if(isFlickering)
        {
            if( (Time.frameCount % flickerSpeed) < (flickerSpeed / 2))
            {
                foreach (Material m in materials)
                {
                    m.color = new Color(m.color.g, m.color.g, m.color.b, flickerAlpha);
                }
            }
            else
            {
                foreach (Material m in materials)
                {
                    m.color = new Color(m.color.g, m.color.g, m.color.b, 1f);
                }
            }
        }

        behaviorTree.Update();
        animator.SetBool("moving", rb.velocity.sqrMagnitude > 0.25f * 0.25f);
    }

    void FixedUpdate() {
        if(transform.up != Vector3.up)
        {
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z)));
        }

        if(canTurn)
        {
            Face();
        }
        else
        {
            StopTurn();
        }

        rb.AddTorque(alpha * Vector3.up, ForceMode.Acceleration);
        if(canDash)
        {
            accel = Vector3.zero;
        }
        else if(canMove)
        {
            Arrive();
        }
        else
        {
            Stop();
        }
        rb.AddForce(accel, ForceMode.Acceleration);
    }

    public bool Target()
    {
        canTurn = true;
        Vector3 diff = player.position - transform.position;
        diff.y = 0;
        return Vector3.Angle(transform.forward, diff) <= 10;
    }
    public void Move()
    {
        //if (audio.isPlaying == false)
        //{
        //    randomer = UnityEngine.Random.Range(0, 5);
        //    audio.clip = steps[randomer];
        //    audio.Play();
        //}
        canMove = true;
    }

    public void Attack()
    {
        animator.SetBool("windupDone", true);
        canTurn = false;
        canMove = false;
        Attack a = (Attack)attackObject.AddComponent<Attack>();
        a.Initialize(lightDamage, Vector3.zero, lightAttackTime, gameObject);
    }
    
    public void HeavyAttack()
    {
        randomer = UnityEngine.Random.Range(0, 1);
        audio.clip = heavyswings[randomer];
        audio.Play();
        animator.SetBool("windupDone", true);
        canTurn = false;
        canMove = false;
        Dash(transform.forward, heavyAttackDistance, heavyAttackTime);
        Attack a = (Attack)attackObject.AddComponent<Attack>();
        a.Initialize(heavyDamage, Vector3.zero, heavyAttackTime, gameObject);
    }

    public void Dash(Vector3 dir, float? dist = null, float? time = null)
    {
        if(!time.HasValue)animator.SetBool("dash", true);
        if (audio.clip != heavyswing1 && audio.clip != heavyswing2)
        {
            audio.clip = dash;
            audio.Play();
        }
        canMove = false;
        canDash = true;
        Vector3 dv = dir * (dist ?? dashDistance)/(time ?? dashTime) - rb.velocity;
        rb.AddForce(dv, ForceMode.VelocityChange);
    }

    private void Arrive()
    {
        if (audio.isPlaying == false && isAlive == true)
        {
            randomer = UnityEngine.Random.Range(0, 5);
            audio.clip = steps[randomer];
            audio.Play();
        }
        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;
        dir = dir.normalized;
        if(dist < targetRadius)
        {
            Stop();
            return;
        }

        float speed = maxSpeed;
        if(dist < slowRadius)
        {
            speed = maxSpeed * dist / slowRadius;
        }

        Vector3 a = dir * speed - rb.velocity;
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
    public void StopDash()
    {
        canDash = false;
        rb.velocity = Vector3.zero;
        animator.SetBool("dash", false);
    }
    public void StopHeavy()
    {
        canDash = false;
        rb.velocity = Vector3.zero;
    }

    private void Face()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up) * Mathf.Deg2Rad;
        float rotationSize = Mathf.Abs(angle);

        float targetOmega = 0;
        if(rotationSize > slowDistance)
        {
            targetOmega = maxOmega * Mathf.Sign(angle);
        }
        
        else
        {
            targetOmega = maxOmega * angle / slowDistance;
        }
        
        float targetAlpha = (targetOmega - rb.angularVelocity.y)/accelTime;
        
        if(Mathf.Abs(targetAlpha) > maxAlpha)
        {
            targetAlpha = Mathf.Sign(targetAlpha) * maxAlpha;
        }
        
        alpha = targetAlpha;
    }

    private void StopTurn()
    {
        if(Mathf.Abs(rb.angularVelocity.y) <= accelTime * maxAlpha)
        {
            alpha = 0;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            alpha = -Mathf.Sign(rb.angularVelocity.y) * maxAlpha;
        }
    }

    private void LightWindup()
    {
        canMove = false;
        canTurn = false;
        animator.SetBool("lightAttack", true);
    }

    private void HeavyWindup()
    {
        audio.clip = heavyswingwindup;
        audio.Play();
        canMove = false;
        animator.SetBool("heavyAttack", true);
    }

    private void OnDamage(float damage)
    {
        if(takingdamage == false && healthStats.CurrentHealth > 0)
        {
            takingdamage = true;
            audio.clip = takesdamage;
            audio.Play();
        }
        HealthBar.fillAmount = healthStats.CurrentHealth / healthStats.MaxHealth;
        if (healthStats.CurrentHealth > 0)
        {
            isFlickering = true;
            foreach (Material m in materials)
            {
                StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Fade);
            }
        }
    }

    private void OnImmunityEnd()
    {
        takingdamage = false;
        isFlickering = false;
        foreach(Material m in materials)
        {
            StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Opaque);
        }
    }

    private void OnDeath(float overkill)
    {
        audio.Stop();
        audio.clip = dies;
        audio.Play();
        musicplayer.GetComponent<bossthemescript>().fadeoutvoid();
        gate.GetComponent<Gate>().Activate();
        isFlickering = false;
        foreach(Material m in materials)
        {
            StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Opaque);
        }
        isAlive = false;

        //drop sword
        GetComponentInChildren<Weapon>().Holder = null;

        //disable animations
        animator.enabled = false;

        //make ragdoll
        rb.constraints = RigidbodyConstraints.None;
        GetComponent<Collider>().enabled = false;
        SetJointsActive(true);

        //you can restart after a few seconds
        //StartCoroutine(FadeOutAndExit());
        
    }

    public IEnumerator FadeOutAndExit()
    {
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(4);
    }

    public void SetJointsActive(bool jointsActive)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            if (rb.tag.Equals("BossJoint"))
            {
                rb.isKinematic = !jointsActive;
                Collider c = rb.gameObject.GetComponent<Collider>();
                if (c != null) c.enabled = jointsActive;
                JointToggler j = rb.gameObject.GetComponent<JointToggler>();
                if (j != null) j.enabled = jointsActive;
            }

        }
    }
}
