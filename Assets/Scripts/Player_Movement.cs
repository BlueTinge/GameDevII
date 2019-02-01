using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic player movement and actions

public class Player_Movement : MonoBehaviour
{
    public Rigidbody Body;
    public Camera Cam;

    public float WalkForce;
    public float MaxSpeed;

    private Quaternion ReferenceFrame;

    // Start is called before the first frame update
    void Start()
    {
        ReferenceFrame = Cam.transform.rotation;
    }

    void FixedUpdate()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            Vector3 moveForce = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Body.AddRelativeForce(moveForce * WalkForce * Time.deltaTime);
            UnityEngine.Debug.Log(Body.velocity);
            UnityEngine.Debug.Log("After: ");
        }

        //max speed: the lazy way
        if (Body.velocity.x > MaxSpeed) Body.velocity = new Vector3(MaxSpeed, Body.velocity.y, Body.velocity.z);
        if (Body.velocity.z > MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, MaxSpeed);
        if (Body.velocity.x < -MaxSpeed) Body.velocity = new Vector3(-MaxSpeed, Body.velocity.y, Body.velocity.z);
        if (Body.velocity.z < -MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, -MaxSpeed);

        UnityEngine.Debug.Log(Body.velocity);
    }

    private void LateUpdate()
    {
        Cam.transform.rotation = ReferenceFrame;
    }
}
