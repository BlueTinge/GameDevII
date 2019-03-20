using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stargaze.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
[RequireComponent(typeof(MeshRenderer))]
public class EyeEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]private float moveTime;
    [SerializeField]private float maxAccel;
    [SerializeField]private float maxSpeed;
    [SerializeField]private float slowRadius;
    [SerializeField]private float targetRadius;
    [SerializeField]private float accelTime;
    [SerializeField]private float maxOmega;
    [SerializeField]protected float maxAlpha;
    [SerializeField]protected float slowDistance;
    [SerializeField]private float minGoalDistance;
    [SerializeField]private float maxGoalDistance;
    [SerializeField]private float windupTime;
    [SerializeField]private float focusTime;
    [SerializeField]private float coolDown;
    [SerializeField]private float range;
    // [SerializeField]private AudioClip chargesound;
    // [SerializeField]private AudioClip shootsound;
    [SerializeField]private GameObject spawnOnDeath;

    private BehaviorTree behaviorTree;
    private EyeCharge laserCharge;
    private HealthStats healthStats;
    private Rigidbody rb;
    private Transform target;
    private Vector3 accel;
    private float alpha;
    private bool canMove;
    private bool canTurn;
    private new MeshRenderer renderer;
    private Color[] colors;
    private float goalDistance;
    private AudioSource audio;
    public AudioClip takesdamagesoundeffect;
    public AudioClip diessoundeffect;

    void Awake()
    {
        laserCharge = GetComponentInChildren<EyeCharge>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        accel = Vector3.zero;
        canMove = true;
        behaviorTree = new BehaviorTree
        (
            new SelectorTask(new ITreeTask[]
            {
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, target, range),
                    new CallTask(() => {goalDistance = Random.Range(minGoalDistance, maxGoalDistance);  return true;}),
                    new WhileTask
                    (
                        new NotTask
                        (
                            new DelayTask(moveTime)
                        ),
                        new BasicMove(this, 0)
                    ),
                    new CallTask(() => {EndMove(); return true;}),
                    new BasicAttack(this, windupTime - focusTime),
                    new CallTask(() => {EndTurn(); return true;}),
                    new DelayTask(coolDown + focusTime)
                }),
                new CallTask(() => {EndMove(); EndTurn(); return true;})
            })
        );
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
        colors = renderer.materials.Select(m => m.color).ToArray();
        healthStats = GetComponent<HealthStats>();
        healthStats.OnDeath = (overkill) => {Die();};
        healthStats.OnDamage = (damage) => {StartCoroutine(TakeDamage());};
        audio = GetComponent<AudioSource>();
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

        if(!canMove) Stop();
        rb.AddForce(accel, ForceMode.Acceleration);

        if(canTurn)
        {
            Face();
        }
        else
        {
            StopTurn();
        }

        rb.AddTorque(alpha * Vector3.up, ForceMode.Acceleration);
    }

    public bool Target()
    {
        return true;
    }
    public void Move()
    {
        canMove = true;
        canTurn = true;
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

    private void Face()
    {
        Vector3 dir = target.position - transform.position;
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

    private void StopTurn()
    {
        if(Mathf.Abs(rb.angularVelocity.y) <= accelTime * maxAlpha)
        {
            alpha = 0;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            alpha = -maxAlpha;
        }
    }

    public void EndMove()
    {
        canMove = false;
    }
    private void EndTurn()
    {
        canTurn = false;
    }

    public void Attack()
    {
        if(!laserCharge.IsTriggered())
        {
            // print("GO");
            // audio.clip = chargesound;
            // audio.Play();
            laserCharge.SetChargeTime(windupTime);
            laserCharge.Trigger();
        }
    }

    private void Die()
    {
        if(spawnOnDeath) Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator TakeDamage()
    {
        audio.clip = takesdamagesoundeffect;
        audio.Play();
        foreach (var v in renderer.materials)
        {
            v.color = Color.red;
        }
        yield return new WaitForSeconds(healthStats.GetImmunity());
        for (int i = 0; i < colors.Length; ++i)
        {
            renderer.materials[i].color = colors[i];
        }
    }
}