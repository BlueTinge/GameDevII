using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//basic player movement and actions

public class PlayerInput : MonoBehaviour
{
    public GameObject CurrentWeapon;
    public Transform PlayerRightHand;
    public GameObject ItemZone;
    public Rigidbody Body;
    public GameObject ReferenceFrame;
    public Camera Cam;

    public float WalkForce;
    public float MaxSpeed;
    public float rotationSpeed;
    public float camRotationSpeed;

    //input axis/sticks
    //separated in case we want specific options for joysticks vs. keyb/mouse
    [HideInInspector]
    public string MoveHoriz = "LeftHoriz";
    [HideInInspector]
    public string MoveVert = "LeftVert";
    [HideInInspector]
    public string CamHoriz = "RightHoriz";
    [HideInInspector]
    public string CamVert = "RightVert";
    [HideInInspector]
    public string LightAttack = "Attack";
    [HideInInspector]
    public string HeavyAttack = "HeavyAttack";
    [HideInInspector]
    public string Item = "Item";

    public long LightCooldown;
    public long HeavyCooldown;
    public long InteractCooldown = 500;

    private Quaternion camRot;
    private Animator animator;

    private Stopwatch lastAttack = new Stopwatch();
    private Stopwatch lastInteract = new Stopwatch();

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
            float pitch = ang.x + (camRotationSpeed * Input.GetAxis(CamVert) * Time.deltaTime);
            if (Cam.transform.eulerAngles.x > 350 && pitch < ang.x) pitch = ang.x;
            if (Cam.transform.eulerAngles.x < 350 && Cam.transform.eulerAngles.x > 85 && pitch > ang.x) pitch = ang.x;
            camRot = Quaternion.Euler(new Vector3(pitch, ang.y + (camRotationSpeed * Input.GetAxis(CamHoriz) * Time.deltaTime), ang.z));
        }
        
        if ((Input.GetButton(LightAttack) || Input.GetAxis(LightAttack) > 0.2) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > LightCooldown))
        {
            animator.SetTrigger("Swing");
            lastAttack.Restart();
            UnityEngine.Debug.Log("Swing Attack");
        }

        if ((Input.GetButton(HeavyAttack)) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > HeavyCooldown))
        {
            animator.SetTrigger("Heavy");
            lastAttack.Restart();
            UnityEngine.Debug.Log("Heavy Attack");
        }

        if (!lastInteract.IsRunning || lastInteract.ElapsedMilliseconds > InteractCooldown) 
        {
            if (Input.GetButton(Item))
            {
                lastInteract.Restart();
                ItemZone.GetComponent<Collider>().enabled = true;
            }
            else
            {
                ItemZone.GetComponent<Collider>().enabled = false;
            }

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

    public void Equip(GameObject newWeapon)
    {
        if (newWeapon == CurrentWeapon) return;

        UnityEngine.Debug.Log("Equipped weapon");

        CurrentWeapon.transform.parent = newWeapon.transform.parent;
        CurrentWeapon.transform.position = new Vector3(CurrentWeapon.transform.position.x, CurrentWeapon.transform.position.y, CurrentWeapon.transform.position.z+0.2f);
        //CurrentWeapon.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        CurrentWeapon.GetComponent<Rigidbody>().isKinematic = false;

        newWeapon.transform.parent = PlayerRightHand;
        newWeapon.transform.localPosition = new Vector3(0, 0, 0);
        newWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        newWeapon.GetComponent<Rigidbody>().isKinematic = true;

        CurrentWeapon = newWeapon;

        ItemZone.GetComponent<Collider>().enabled = false;
    }
}
