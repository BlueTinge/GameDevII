using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gate : Activatable
{
    public Animator Animator;

    void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public override void Activate()
    {
        Animator.SetBool("Raised", true);
    }

    public override void Deactivate()
    {
        Animator.SetBool("Raised", false);
    }

    public override bool GetIsActivated()
    {
        return Animator.GetBool("Raised");
    }
}
