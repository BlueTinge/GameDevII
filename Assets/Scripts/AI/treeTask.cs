using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stargaze.AI
{
    public enum TaskState
    {
        ready,
        continuing,
        failure,
        failureImmediate,
        success,
        successImmediate
    };
    
    public interface ITreeTask
    {
        TaskState state{get;}
        IEnumerable Update();
        void Reset();
    }

    public class SequenceTask : ITreeTask
    {
        private ITreeTask[] children;
        private TaskState lastState;

        public TaskState state {get; private set;}

        public SequenceTask(IList<ITreeTask> tasks)
        {
            children = tasks.ToArray();
        }

        public IEnumerable Update()
        {
            state = TaskState.continuing;
            foreach (ITreeTask task in children)
            {
                foreach(var _ in task.Update())
                {
                    yield return null;
                }
                lastState = task.state;
                switch (lastState)
                {
                case TaskState.success:
                    yield return null;
                    break;
                case TaskState.successImmediate:
                    break;
                case TaskState.failure:
                    state = TaskState.failure;
                    yield break;
                    break;
                case TaskState.failureImmediate:
                    state = TaskState.failureImmediate;
                    yield break;
                    break;
                default:
                    state = TaskState.failure;
                    yield break;
                    break;
                }
            }
            state = lastState;
        }

        public void Reset()
        {
            state = TaskState.ready;
            lastState = TaskState.ready;
            foreach(ITreeTask task in children)
            {
                task.Reset();
            }
        }
    }

    public class SelectorTask : ITreeTask
    {
        private ITreeTask[] children;
        private TaskState lastState;

        public TaskState state {get; private set;}

        public SelectorTask(IList<ITreeTask> tasks)
        {
            children = tasks.ToArray();
        }

        public IEnumerable Update()
        {
            state = TaskState.continuing;
            foreach (ITreeTask task in children)
            {
                foreach(var _ in task.Update())
                {
                    yield return null;
                }
                lastState = task.state;
                switch (lastState)
                {
                case TaskState.success:
                    state = TaskState.success;
                    yield break;
                    break;
                case TaskState.successImmediate:
                    state = TaskState.successImmediate;
                    yield break;
                    break;
                case TaskState.failure:
                    yield return null;
                    break;
                case TaskState.failureImmediate:
                    break;
                default:
                    state = TaskState.failure;
                    yield break;
                    break;
                }
            }
            state = lastState;
        }

        public void Reset()
        {
            state = TaskState.ready;
            lastState = TaskState.ready;
            foreach(ITreeTask task in children)
            {
                task.Reset();
            }
        }
    }

    public class MuteTask : ITreeTask
    {
        ITreeTask child;
        public TaskState state{get; private set;}

        public MuteTask(ITreeTask task)
        {
            child = task;
        }

        public IEnumerable Update()
        {
            state = TaskState.continuing;
            foreach(var _ in child.Update())
            {
                yield return null;
            }

            if(child.state == TaskState.successImmediate || child.state == TaskState.failureImmediate)
            {
                state = TaskState.successImmediate;
            }
            else
            {
                state = TaskState.success;
            }
        }

        public void Reset()
        {
            state = TaskState.ready;
            child.Reset();
        }
    }

    public class NotTask : ITreeTask
    {
        ITreeTask child;
        public TaskState state{get; private set;}

        public NotTask(ITreeTask task)
        {
            child = task;
        }

        public IEnumerable Update()
        {
            state = TaskState.continuing;
            foreach(var _ in child.Update())
            {
                yield return null;
            }

            switch(child.state)
            {
            case TaskState.success:
                state = TaskState.failure;
                break;
            case TaskState.successImmediate:
                state = TaskState.failureImmediate;
                break;
            case TaskState.failure:
                state = TaskState.success;
                break;
            case TaskState.failureImmediate:
                state = TaskState.successImmediate;
                break;
            default:
                state = TaskState.failure;
                break;
            }
        }

        public void Reset()
        {
            state = TaskState.ready;
            child.Reset();
        }
    }

    public class DelayTask : ITreeTask
    {
        private float startTime;
        private float duration;
        public TaskState state{get; private set;}
        
        public DelayTask(float duration)
        {
            this.duration = duration;
        }

        public IEnumerable Update()
        {
            if(state == TaskState.ready)
            {
                startTime = Time.time;
                state = TaskState.continuing;
            }
            while(Time.time - startTime < duration)
            {
                yield return null;
            }
            state = TaskState.success;
        }

        public void Reset()
        {
            state = TaskState.ready;
        }
    }

    public class BehaviorTree
    {
        private ITreeTask root;
        private IEnumerator state;
        public BehaviorTree(ITreeTask root)
        {
            this.root = root;
        }

        public void Update()
        {
            if(root.state == TaskState.ready)
            {
                state = root.Update().GetEnumerator();
            }
            state.MoveNext();
            if(root.state == TaskState.success || root.state == TaskState.failure)
            {
                root.Reset();
            }
            else if(root.state == TaskState.successImmediate || root.state == TaskState.failureImmediate)
            {
                root.Reset();
                state = root.Update().GetEnumerator();
            }
        }
    }
}