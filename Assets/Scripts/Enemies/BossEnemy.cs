using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

class BossMoveBack : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Moving back.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BossMoveUp : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Moving up.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BossMoveSideways : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Moving sideways.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BossAttack : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Attacking.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BossCharge : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Charging.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BossMagic : ITreeTask
{
    public TaskState state{get; private set;}

    public IEnumerable Update()
    {
        Debug.Log("Magicing.");
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthStats))]
public class BossEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float closeCombatRange;
    [SerializeField] private float[] closeWeights;
    [SerializeField] private float[] farWeights;
    private Transform player;
    private Rigidbody rb;
    private HealthStats healthStats;
    private BehaviorTree behaviorTree;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        healthStats = GetComponent<HealthStats>();
        healthStats.OnDeath = (overkill) => {Destroy(gameObject);};

        behaviorTree = new BehaviorTree
        (
            new SelectorTask(new ITreeTask[]
            {
                //Artificial delay to reduce spam
                new NotTask(new DelayTask(0.5f)),
                new SequenceTask(new ITreeTask[]
                {
                    new CloseTo(transform, player, closeCombatRange),
                    new RandomSelectTask(closeWeights, new ITreeTask[]
                    {
                        new BossMoveBack(),
                        new BossMoveSideways(),
                        new BossAttack(),
                    })
                }),
                new RandomSelectTask(farWeights, new ITreeTask[]
                {
                    new BossMoveUp(),
                    new BossMoveSideways(),
                    new BossCharge(),
                    new BossMagic()
                })
            })
        );
    }

    void Update()
    {
        behaviorTree.Update();
    }

    public bool Target(){return true;}
    public void Move(){}

    public void Attack(){}
}
