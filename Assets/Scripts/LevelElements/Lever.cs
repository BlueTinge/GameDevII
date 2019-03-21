using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Lever : Activatable
{
    [Tooltip("Set grates, wires, other levers, etc. that are activated by this lever here:")]
    public Activatable[] Connected;

    [Space(10)]
    [Tooltip("If true, lever will start in active position")]
    public bool StartActive = false;
    [Tooltip("If false, lever stays active after deactivation (e.g. cannot be toggled)")]
    public bool CanDeactivate = true;

    private Animator Animator;
    private bool isChanging = false;

    void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (StartActive) Activate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ItemZone"))
        {
            if (this.GetIsActivated()) this.Deactivate();
            else                       this.Activate(); 
        }
    }

    public override bool GetIsActivated()
    {
        //invariant: lowered is active
        return !Animator.GetBool("Raised");
    }

    public override void Activate()
    {
        //invariant: lowered is active
        Animator.SetBool("Raised", false);

        //prevent inf loops
        if (isChanging) return;
        isChanging = true;

        foreach (Activatable a in Connected)
        {
            a.Activate();
        }

        isChanging = false;
    }

    public override void Deactivate()
    {
        if (CanDeactivate)
        {
            //invariant: raised is inactive
            Animator.SetBool("Raised", true);

            //prevent inf loops
            if (isChanging) return;
            isChanging = true;

            foreach (Activatable a in Connected)
            {
                a.Deactivate();
            }

            isChanging = false;
        }
    }
}
