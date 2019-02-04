using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//basic player movement and actions

public class Player_Movement : MonoBehaviour
{
    public Rigidbody Body;
    public GameObject ReferenceFrame;
    public Camera Cam;

    public float WalkForce;
    public float MaxSpeed;
    public float rotationSpeed;
    public float camRotationSpeed;

    //input axis/sticks
    //separated in case we want specific options for joysticks vs. keyb/mouse
    public string MoveHoriz = "LeftHoriz";
    public string MoveVert = "LeftVert";
    public string CamHoriz = "RightHoriz";
    public string CamVert = "RightVert";

    public long SwingCooldown;

    private Quaternion camRot;
    private Animator animator;

    private Stopwatch lastAttack = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        camRot = ReferenceFrame.transform.rotation;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetAxis(CamHoriz) != 0 || Input.GetAxis(CamVert) != 0)
        {
            Vector3 ang = camRot.eulerAngles;
            camRot = Quaternion.Euler(new Vector3(ang.x + (camRotationSpeed * Input.GetAxis(CamVert) * Time.deltaTime), ang.y + (camRotationSpeed * Input.GetAxis(CamHoriz) * Time.deltaTime), ang.z));
        }
        
        if (Input.GetButton("Attack") && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > SwingCooldown))
        {
            animator.SetTrigger("Swing");
            lastAttack.Restart();
        }

        
    }

    void FixedUpdate()
    {
        if (Input.GetAxis(MoveVert) != 0 || Input.GetAxis(MoveHoriz) != 0)
        {
            Vector3 moveForce = new Vector3(Input.GetAxis(MoveHoriz), 0, Input.GetAxis(MoveVert));
            Quaternion moveDirection =  Quaternion.Euler(0, camRot.eulerAngles.y, 0);

            Body.AddForce(moveDirection * moveForce * WalkForce /* * Time.deltaTime*/);

            Vector3 angFrom = Body.rotation.eulerAngles;
            Vector3 angTo = Quaternion.LookRotation(moveDirection * moveForce).eulerAngles;

            float sign = 0;
            if (angTo.y > angFrom.y)
            {
                if (angTo.y - angFrom.y >= 180) sign = -1;
                else sign = 1;
            }
            else
            {
                if (angFrom.y - angTo.y >= 180) sign = 1;
                else sign = -1;
            }

            if (Mathf.Abs(angFrom.y - angTo.y) < rotationSpeed * moveForce.magnitude)
            {
                Body.MoveRotation(Quaternion.Euler(new Vector3(angTo.x, angTo.y, angTo.z)));
            }
            else Body.MoveRotation(Quaternion.Euler(new Vector3(angFrom.x, angFrom.y + (sign * rotationSpeed * moveForce.magnitude), angFrom.z)));
            //UnityEngine.Debug.Log(Body.velocity);
            //UnityEngine.Debug.Log("After: ");
        }

        //max speed: the lazy way
        if (Body.velocity.x > MaxSpeed) Body.velocity = new Vector3(MaxSpeed, Body.velocity.y, Body.velocity.z);
        if (Body.velocity.z > MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, MaxSpeed);
        if (Body.velocity.x < -MaxSpeed) Body.velocity = new Vector3(-MaxSpeed, Body.velocity.y, Body.velocity.z);
        if (Body.velocity.z < -MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, -MaxSpeed);

        //UnityEngine.Debug.Log(Body.velocity);
        ReferenceFrame.transform.rotation = camRot;
    }

    private void LateUpdate()
    {
        ReferenceFrame.transform.rotation = camRot;
    }
}
