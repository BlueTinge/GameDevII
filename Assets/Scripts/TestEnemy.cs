using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Stargaze.AI;

class CloseTo : ITreeTask
{
    private Transform self;
    private Transform target;
    private float rangeSq;
    public TaskState state{get; private set;}
    
    public CloseTo(Transform self, Transform target, float range)
    {
        this.self = self;
        this.target = target;
        rangeSq = range * range;
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        if((target.position - self.position).sqrMagnitude <= rangeSq)
        {
            state = TaskState.successImmediate;
        }
        else
        {
            state = TaskState.failureImmediate;
        }
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class TurnColor: ITreeTask
{
    private Renderer renderer;
    private Color color;
    public TaskState state{get; private set;}
    
    public TurnColor(Renderer self, Color color)
    {
        renderer = self;
        this.color = color;
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        renderer.material.color = color;
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

public class TestEnemy : MonoBehaviour
{
    private BehaviorTree behaviorTree;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();
        Renderer renderer = GetComponent<Renderer>();
        if(player)
        {
            behaviorTree = new BehaviorTree
            (
                new SelectorTask(new ITreeTask[]
                {
                    new SequenceTask(new ITreeTask[]{
                        new CloseTo(transform, player.transform, 5),
                        new TurnColor(renderer, Color.yellow),
                        new DelayTask(1),
                        new CloseTo(transform, player.transform, 5),
                        new TurnColor(renderer, Color.red)
                    }),
                    new TurnColor(renderer, Color.green)
                })
            );
        }
        else
        {
            behaviorTree = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        behaviorTree.Update();
    }
}
