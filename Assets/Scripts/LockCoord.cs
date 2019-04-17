using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class LockCoord : MonoBehaviour
{
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;

    private Vector3 startPos;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = rb.position;
    }
    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        pos.x = lockX ? startPos.x : pos.x;
        pos.y = lockY ? startPos.y : pos.y;
        pos.z = lockZ ? startPos.z : pos.z;
    }
}
