using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

