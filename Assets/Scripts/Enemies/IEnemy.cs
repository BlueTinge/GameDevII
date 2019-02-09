using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stargaze.AI;

class BasicTarget : ITreeTask
{
    public TaskState state{get; private set;}
    private IEnemy self;

    public BasicTarget(IEnemy self)
    {
        this.self = self;
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        self.Target();
        state = TaskState.success;
        yield break;
    }

    public void Reset()
    {
        state = TaskState.ready;
    }
}

class BasicAttack : ITreeTask
{
    public TaskState state{get; private set;}
    private IEnemy self;
    private DelayTask delay;

    public BasicAttack(IEnemy self, float attackTime)
    {
        this.self = self;
        delay = new DelayTask(attackTime);
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        state = TaskState.continuing;
        self.Attack();
        foreach(object _ in delay.Update())
        {
            yield return null;
        }
        state = TaskState.success;
    }

    public void Reset()
    {
        delay.Reset();
        state = TaskState.ready;
    }
}

class BasicMove : ITreeTask
{
    public TaskState state{get; private set;}
    private IEnemy self;
    private DelayTask delay;

    public BasicMove(IEnemy self, float moveTime)
    {
        this.self = self;
        delay = new DelayTask(moveTime);
        state = TaskState.ready;
    }

    public IEnumerable Update()
    {
        state = TaskState.continuing;
        self.Move();
        foreach(object _ in delay.Update())
        {
            yield return null;
        }
        state = TaskState.success;
    }

    public void Reset()
    {
        delay.Reset();
        state = TaskState.ready;
    }
}

public interface IEnemy
{
    void Target();
    void Move();
    void Attack();
}