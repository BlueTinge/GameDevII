using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState { IDLE, WALKING, DASHING, LIGHT_ATTACKING, HEAVY_ATTACKING, BOUNCE_BACK, HURT, DEATH}

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
    public ParticleSystem HealParticleSystem;

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
    public bool AnyJournal = false;
    public bool JournalColllect1 = false;
    public bool JournalColllect2 = false;
    public bool JournalColllect3 = false;
    public bool JournalColllect4 = false;
    public bool JournalColllect5 = false;

    public int NumPotions = 0;
    public float HealAmount = 10;

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
    public string HealButton = "Heal";
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
    private Stopwatch lastHeal = new Stopwatch();

    public AudioSource audio;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip footstep4;
    public AudioClip footstep5;
    public AudioClip footstep6;
    public AudioClip secondswing;
    public AudioClip heavyswing;
    public AudioClip healsound;
    public AudioClip failedhealsound;
    public AudioClip deathsound;
    public GameObject damagesound;
    public GameObject dashsound;
    public Transform healtransform;
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
        if (UIManager.isInputEnabled)
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

                StartCoroutine(TargetNearestEnemy());
                UnityEngine.Debug.Log("Swing Attack");
            }

            if ((State == PlayerState.IDLE || State == PlayerState.WALKING) && (Input.GetButtonDown(HeavyAttackButton)) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > HeavyCooldown))
            {
                PlayerAnimator.SetTrigger("Heavy");
                lastAttack.Restart();
                State = PlayerState.HEAVY_ATTACKING;
                //Body.velocity = new Vector3(0, 0, 0); //turn off (or make coroutine) for skid
                audio.clip = heavyswing;
                audio.Play();
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

            if ((!lastHeal.IsRunning || lastHeal.ElapsedMilliseconds > InteractCooldown) && Input.GetButton(HealButton))
            {
                lastHeal.Restart();
                StartCoroutine(UsePotion());
            }
        }
    }

    //Check for player input for physics stuff every fixed update
    void FixedUpdate()
    {
        if (UIManager.isInputEnabled)
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
                            randomer = UnityEngine.Random.Range(0, 5);
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
    }

    private void LateUpdate()
    {
        ReferenceFrame.transform.rotation = camRot;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (State == PlayerState.DASHING)
        {
            print("dashwall");
            //This code prevents dashing into objects to send them flying, but is sloppy and may introduce bugs
            //TODO: edit this code so that you can't stop every rb from moving just by dashing into it 
            if (collision.rigidbody != null) collision.rigidbody.velocity = new Vector3(0, 0, 0);
            Body.velocity = new Vector3(0, 0, 0);
        }
    }

    public IEnumerator Dash(Vector3 Direction)
    {
        State = PlayerState.DASHING;
        PlayerAnimator.SetBool("IsDashing", true);
        Instantiate(dashsound);

        Body.velocity = Direction.normalized * DashSpeed;
        UnityEngine.Debug.Log(Body.velocity);
        PlayerHealth.isImmune = true;
        yield return new WaitForSeconds(DashTime);
        PlayerHealth.isImmune = false;
        Body.velocity = new Vector3(0, 0, 0);
        PlayerAnimator.SetBool("IsDashing", false);
        yield return new WaitForSeconds(DashRecoveryTime);
        State = PlayerState.IDLE;
    }

    void playhealsound(AudioClip theclip)
    {
        audio = healtransform.GetComponent<AudioSource>();
        //audio = transform.GetChild(3).GetComponent<AudioSource>();
        audio.clip = theclip;
        audio.Play();
        audio = GetComponent<AudioSource>();
    }

    //Attempt to use potion
    public IEnumerator UsePotion()
    {
        if(NumPotions > 0 && PlayerHealth.CurrentHealth < PlayerHealth.MaxHealth)
        {
            playhealsound(healsound);
            NumPotions--;
            PlayerHealth.CurrentHealth = Mathf.Min(PlayerHealth.CurrentHealth + HealAmount, PlayerHealth.MaxHealth);
            HealParticleSystem.Play();
            yield return new WaitForSeconds(HealParticleSystem.main.startLifetime.constant+1);
            HealParticleSystem.Stop();
        }
        else
        {
            //TODO: play sound for when you have no potions
            playhealsound(failedhealsound);
        }

        yield return null;
    }

    //Snap to nearest enemy and attack
    //Both turn towards it and move towards it, depending on tweakables.
    public IEnumerator TargetNearestEnemy()
    {
        //find closest enemy within range
        GameObject closestEnemy = null;
        float closestEnemyDistance = float.PositiveInfinity;

        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (ItemZone.GetComponent<Collider>().bounds.Contains(e.transform.position))
            {
                if(Vector3.Distance(transform.position, e.transform.position) < closestEnemyDistance)
                {
                    closestEnemy = e;
                    closestEnemyDistance = Vector3.Distance(transform.position, e.transform.position);
                }
            }
        }

        if(closestEnemy != null)
        {

        }
        yield return null;
    }

    //called when sword strike against furniture, etc. causes player to bounce back
    public IEnumerator BounceBack(Vector3 knockback)
    {
        if(State == PlayerState.LIGHT_ATTACKING || State == PlayerState.HEAVY_ATTACKING)
        {
            State = PlayerState.BOUNCE_BACK;
            PlayerAnimator.SetTrigger("Bounce");
            GetComponent<Rigidbody>().AddForce(knockback);

            yield return new WaitForSeconds(.5f);

            if (State == PlayerState.BOUNCE_BACK) State = PlayerState.IDLE;
        }
        yield return null;
    }

    public void OnDamage(float damage)
    {
        // HealthBarSlider.value -= damage;
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

    public IEnumerator MakeLightAttack(AnimationEvent e)
    {
        float ttl = e.floatParameter;
        bool isSecondSwing = false;
        if (e.stringParameter == "LightSwing2") isSecondSwing = true;
        GetComponent<Equipment>().CurrentWeapon.GetComponent<Weapon>().MakeLightAttack(ttl, isSecondSwing);
        yield return new WaitForSeconds(ttl);
        if (State == PlayerState.LIGHT_ATTACKING) State = PlayerState.IDLE;
        if (isSecondSwing == true)
        {
            audio.Stop();
            audio.clip = secondswing;
            audio.Play();
        }
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
                AnyJournal = true;
                img.text = other.GetComponent<TextHolder>().TextData.ToString();
                Destroy(other.gameObject);
                txt.text = "";
            }
        }
    }
}

