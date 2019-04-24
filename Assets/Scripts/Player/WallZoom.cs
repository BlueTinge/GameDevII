using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallZoom : MonoBehaviour
{
    [SerializeField] private float offset = 0.3f;
    private Vector3 startDir;
    private float startDist;
    private Transform parent;

    void Start()
    {
        parent = transform.parent;
        Vector3 startPos = transform.localPosition;
        startDist = startPos.magnitude;
        startDir = startPos.normalized;

    }

    void Update()
    {
        RaycastHit right;
        RaycastHit left;
        int mask = LayerMask.GetMask("Ignore Raycast", "PlayerLayer", "Item");
        mask = ~mask;
        bool rightHit = Physics.Raycast(parent.position + parent.right * offset, parent.TransformDirection(startDir), out right, startDist, mask);
        bool leftHit = Physics.Raycast(parent.position - parent.right * offset, parent.TransformDirection(startDir), out left, startDist, mask);
        Debug.DrawRay(parent.position + parent.right, parent.TransformDirection(startDir) * startDist, Color.green);
        Debug.DrawRay(parent.position - parent.right, parent.TransformDirection(startDir) * startDist, Color.green);
        float dist;
        if(rightHit && leftHit)
        {
            float leftDist = left.distance;
            float rightDist = right.distance;
            dist = Mathf.Min(leftDist, rightDist);
        }
        else if(rightHit)
        {
            dist = right.distance;
        }
        else if(leftHit)
        {
            dist = left.distance;
        }
        else
        {
            dist = startDist;
        }

        transform.localPosition = startDir * dist;
    }
}
