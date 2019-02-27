using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState { IDLE, WALKING, DASHING, LIGHT_ATTACKING, HEAVY_ATTACKING, HURT, DEATH}

//
//basic player movement and actions
//also acts as controller for other modules (eg HealthStats)

[RequireComponent(typeof(Equipment))]
[RequireComponent(typeof(HealthStats))]
public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;
    public GameObject ReferenceFrame;
    public Camera Cam;
    public Animator PlayerAnimator;

    public float WalkForce;
    public float MaxSpeed;
    public float rotationSpeed;
    public float camRotationSpeed;
    public float DashSpeed;
    [Tooltip("Time dash movement takes (does not count recovery)")]
    public float DashTime;
    [Tooltip("Time dash recovery takes (period after dash finishes)")]
    public float DashRecoveryTime;
    public long DashCooldown = 500;

    public long LightCooldown;
    public long HeavyCooldown;
    public float HeavyAttackForce;
    public long InteractCooldown = 500;

    public Slider HealthBarSlider;
    public Text img;
    public Text txt;
    public bool JournalColllect1 = false;


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
    public string LightAttackButton = "Attack";
    [HideInInspector]
    public string HeavyAttackButton = "HeavyAttack";
    [HideInInspector]
    public string ItemButton = "Item";
    [HideInInspector]
    public string DashButton = "Dash";
    [HideInInspector]
    public string Trigger = "Trigger";

    public PlayerState State = PlayerState.IDLE;

    private Transform PlayerRightHand;
    private GameObject ItemZone;
    private HealthStats PlayerHealth;

    private Quaternion camRot;

    private Stopwatch lastAttack = new Stopwatch();
    private Stopwatch lastDash = new Stopwatch();
    private Stopwatch lastInteract = new Stopwatch();

    public AudioSource audio;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip footstep4;
    public AudioClip footstep5;
    public AudioClip footstep6;
    public AudioClip deathsound;
    public GameObject damagesound;
    public GameObject dashsound;
    public List<AudioClip> steps = new List<AudioClip>();
    int randomer;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRightHand = GetComponent<Equipment>().DomHand;
        ItemZone = GetComponent<Equipment>().ItemZone;

        camRot = ReferenceFrame.transform.rotation;

        PlayerHealth = GetComponent<HealthStats>();
        PlayerHealth.OnDeath = OnDeath;
        PlayerHealth.OnDamage = OnDamage;

        audio = GetComponent<AudioSource>();
        steps.Add(footstep1);
        steps.Add(footstep2);
        steps.Add(footstep3);
        steps.Add(footstep4);
        steps.Add(footstep5);
        steps.Add(footstep6);

        img.gameObject.SetActive(false);
    }

    //Check for player input for non-physics stuff every update
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

        if ((State == PlayerState.IDLE || State == PlayerState.WALKING || State == PlayerState.LIGHT_ATTACKING) && (Input.GetButtonDown(LightAttackButton) || Input.GetAxis(Trigger) > 0.2) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > LightCooldown))
        {
            PlayerAnimator.SetTrigger("Swing");
            lastAttack.Restart();
            State = PlayerState.LIGHT_ATTACKING;
            //Body.velocity = new Vector3(0, 0, 0); //turn off (or make coroutine) for skid

            UnityEngine.Debug.Log("Swing Attack");
        }

        if ((State == PlayerState.IDLE || State == PlayerState.WALKING) && (Input.GetButtonDown(HeavyAttackButton)) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > HeavyCooldown))
        {
            PlayerAnimator.SetTrigger("Heavy");
            lastAttack.Restart();
            State = PlayerState.HEAVY_ATTACKING;
            //Body.velocity = new Vector3(0, 0, 0); //turn off (or make coroutine) for skid

            UnityEngine.Debug.Log("Heavy Attack");

        }

        //TODO move somewhere better? Invoke method in Equipment, maybe?
        if (!lastInteract.IsRunning || lastInteract.ElapsedMilliseconds > InteractCooldown)
        {
            if (Input.GetButton(ItemButton))
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

    //Check for player input for physics stuff every fixed update
    void FixedUpdate()
    {

        //whitelist of states we can move in
        if (State == PlayerState.IDLE || State == PlayerState.WALKING)
        {
            if (Input.GetAxis(MoveVert) != 0 || Input.GetAxis(MoveHoriz) != 0)
            {
                Vector3 inputForce = new Vector3(Input.GetAxis(MoveHoriz), 0, Input.GetAxis(MoveVert));
                Quaternion moveDirection = Quaternion.Euler(0, camRot.eulerAngles.y, 0);

                if ((Input.GetButton(DashButton) || Input.GetAxis(Trigger) < -0.2) && (!lastDash.IsRunning || lastDash.ElapsedMilliseconds > DashCooldown) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > LightCooldown))
                {
                    lastDash.Restart();
                    UnityEngine.Debug.Log("Dash");
                    StartCoroutine(Dash(moveDirection * inputForce));
                }
                else
                {
                    State = PlayerState.WALKING;

                    //Move the player in the direction of the control stick relative to the camera
                    //TODO: Evaluate whether player should be moved via forces, or just have its velocity modified directly.
                    Body.AddForce(moveDirection * inputForce * WalkForce /* * Time.deltaTime*/);

                    //WHENYOUWALKING

                    if (audio.isPlaying == false)
                    {
                        randomer = Random.Range(0, 5);
                        audio.clip = steps[randomer];
                        audio.Play();
                    }

                    //Find amount and direction player should rotate to/in
                    Vector3 angFrom = Body.rotation.eulerAngles;
                    Vector3 angTo = Quaternion.LookRotation(moveDirection * inputForce).eulerAngles;

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

                    if (Mathf.Abs(angFrom.y - angTo.y) < rotationSpeed * inputForce.magnitude)
                    {
                        Body.MoveRotation(Quaternion.Euler(new Vector3(angTo.x, angTo.y, angTo.z)));
                    }
                    else Body.MoveRotation(Quaternion.Euler(new Vector3(angFrom.x, angFrom.y + (sign * rotationSpeed * inputForce.magnitude), angFrom.z)));
                }
            }
            else State = PlayerState.IDLE;

            //max speed: the lazy way
            //note that this does not apply in non-movement states (e.g. you can go flying if hurt, or go faster if dashing)
            if (State == PlayerState.IDLE || State == PlayerState.WALKING)
            {
                if (Body.velocity.x > MaxSpeed) Body.velocity = new Vector3(MaxSpeed, Body.velocity.y, Body.velocity.z);
                if (Body.velocity.z > MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, MaxSpeed);
                if (Body.velocity.x < -MaxSpeed) Body.velocity = new Vector3(-MaxSpeed, Body.velocity.y, Body.velocity.z);
                if (Body.velocity.z < -MaxSpeed) Body.velocity = new Vector3(Body.velocity.x, Body.velocity.y, -MaxSpeed);
            }
        }
    }

    private void LateUpdate()
    {
        ReferenceFrame.transform.rotation = camRot;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (State == PlayerState.DASHING)
        {
            //This code prevents dashing into objects to send them flying, but is sloppy and may introduce bugs
            //TODO: edit this code so that you can't stop every rb from moving just by dashing into it 
            if (collision.rigidbody != null) collision.rigidbody.velocity = new Vector3(0, 0, 0);
            Body.velocity = new Vector3(0, 0, 0);
        }
    }

    public IEnumerator Dash(Vector3 Direction)
    {
        State = PlayerState.DASHING;

        Instantiate(dashsound);

        Body.velocity = Direction.normalized * DashSpeed;
        UnityEngine.Debug.Log(Body.velocity);
        PlayerHealth.isImmune = true;
        yield return new WaitForSeconds(DashTime);
        PlayerHealth.isImmune = false;
        Body.velocity = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(DashRecoveryTime);
        State = PlayerState.IDLE;
    }

    public void OnDamage(float damage)
    {
        HealthBarSlider.value -= damage;
        if (State != PlayerState.DEATH && damage > 0)
        {
            State = PlayerState.HURT;
            PlayerAnimator.SetTrigger("Damage");
            PlayerAnimator.ResetTrigger("Idle");
            Invoke("SetStateIdle", 0.5f);
            Instantiate(damagesound);


        }
    }

    public void OnDeath(float overkill)
    {
        State = PlayerState.DEATH;
        PlayerAnimator.SetTrigger("Death");

        audio.clip = deathsound;
        audio.Play();

        foreach (Attack a in GetComponent<Equipment>().CurrentWeapon.GetComponents<Attack>()) { Destroy(a); }
    }


    //ANIMATION EVENTS used for controlling the SPECIFIC moment in an animation where an attack is made (as opposed to player input which controls state)
    //Just delegate to weapon
    //Note that they are called at the specific point in an animation where the attack is made
    //Should these be refactored into a specific Player Attack Controller class?
    //Note that if ttls vary by weapon (not just weapon class or animation) this will need to be edited
    //Note that recovery animations should probably have a "set state idle" event after the attack finishes, as opposed to cramming it in the "make attack" methods

    public IEnumerator MakeLightAttack(float ttl)
    {
        GetComponent<Equipment>().CurrentWeapon.GetComponent<Weapon>().MakeLightAttack(ttl);
        yield return new WaitForSeconds(ttl);
        if (State == PlayerState.LIGHT_ATTACKING) State = PlayerState.IDLE;
    }

    public IEnumerator MakeHeavyAttack(float ttl)
    {
        GetComponent<Equipment>().CurrentWeapon.GetComponent<Weapon>().MakeHeavyAttack(ttl);
        Body.AddRelativeForce(Vector3.forward * HeavyAttackForce);
        yield return new WaitForSeconds(ttl);
        if (State == PlayerState.HEAVY_ATTACKING)
        {
            State = PlayerState.IDLE;
            //Body.velocity = new Vector3(0, 0, 0); //enable if it should stop abruptly after attack ends
        }
    }

    private void SetStateIdle()
    {
        if (State != PlayerState.DEATH)
        {
            State = PlayerState.IDLE;
            PlayerAnimator.SetTrigger("Idle");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Journal")
        {
            txt.text = "Press E to pick up";
            if (Input.GetKeyDown(KeyCode.E))
            {
                JournalColllect1 = true;
                img.text = other.GetComponent<TextHolder>().TextData.ToString();
                Destroy(other.gameObject);
                txt.text = "";
            }
        }
    }
}

