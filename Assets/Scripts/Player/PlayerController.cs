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
    public Collider PlayerCollider;

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

    [Tooltip("If player's distance to enemy is less than this, targeting is attempted")]
    public float Target_Range = 3f;
    [Tooltip("If player's angle to enemy is less than this, targeting is attempted")]
    public float Target_Angular_Range = 180;
    [Tooltip("# of fixed frames to attempt targeting for. 60fps")]
    public int NumFramesTarget = 10;
    [Tooltip("Speed to rotate when targeting")]
    public float TargetRotationSpeed = 8f;
    [Tooltip("Distance must be greater than this to attempt move targeting")]
    public float Target_Min_Range = 1.5f;
    [Tooltip("Increment to move (per frame) when targeting")]
    public float Target_Move_Increment = .1f;

    public int FlickerSpeed = 20;
    public float FlickerAlpha = 0.5f;
    private bool isFlickering = false;

    public bool CamDriftEnabled = false;
    public float CamSnapIncrement;
    public float CamDriftRange;
    public float CamCooldown;
    private bool CamIsDrifting = false;

    public Slider HealthBarSlider;
    public Text img;
    public Text txt;
    public bool AnyJournal = false;
    public bool JournalColllect1 = false;
    public bool JournalColllect2 = false;
    public bool JournalColllect3 = false;
    public bool JournalColllect4 = false;
    public bool JournalColllect5 = false;
    public bool JournalColllect6 = false;
    public bool JournalColllect7 = false;
    public bool JournalColllect8 = false;
    public bool JournalColllect9 = false;
    public bool JournalColllect10 = false;


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
    private GameObject ItemZoneArea;
    private HealthStats PlayerHealth;
    private Material[] PlayerMaterials;

    private Quaternion camRot;

    private Stopwatch lastAttack = new Stopwatch();
    private Stopwatch lastDash = new Stopwatch();
    private Stopwatch lastInteract = new Stopwatch();
    private Stopwatch lastHeal = new Stopwatch();
    private Stopwatch lastCamMovement = new Stopwatch();

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
    public AudioClip clangsound;
    public GameObject damagesound;
    public GameObject dashsound;
    public Transform healtransform;
    public List<AudioClip> steps = new List<AudioClip>();
    int randomer;

    private void Awake()
    {
        if (Manager.GetWeapon() != null)
        {
            print(Manager.GetWeapon());
            GameObject oldWeapon = GetComponent<Equipment>().CurrentWeapon;
            Transform t = oldWeapon.transform;
            GameObject newWeapon = Instantiate(Manager.GetWeapon(), t.position, t.rotation, t.parent);
            oldWeapon.transform.position = new Vector3(-10000, -10000, -100000);
            GetComponent<Equipment>().Equip(newWeapon);
            Destroy(oldWeapon);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRightHand = GetComponent<Equipment>().DomHand;
        ItemZone = GetComponent<Equipment>().ItemZone;
        ItemZoneArea = Instantiate(ItemZone, ItemZone.transform);
        ItemZoneArea.tag = "ItemZoneArea";
        ItemZoneArea.GetComponent<Collider>().enabled = true;

        camRot = ReferenceFrame.transform.rotation;

        PlayerHealth = GetComponent<HealthStats>();
        PlayerHealth.OnDeath = OnDeath;
        PlayerHealth.OnDamage = OnDamage;
        PlayerHealth.OnImmunityEnd = OnImmunityEnd;

        NumPotions = Manager.GetNumPotions();
        if (Manager.GetPlayerHealth() > 0) PlayerHealth.CurrentHealth = Manager.GetPlayerHealth();
        if (HealthBarSlider != null)
        {
            HealthBarSlider.value = PlayerHealth.CurrentHealth;
        }
        else UnityEngine.Debug.LogWarning("HealthBarSlider not set in player");

        List<Material> materials = new List<Material>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            foreach (Material m in r.materials)
            {
                if (m.HasProperty(Shader.PropertyToID("_Color")))
                {
                    materials.Add(m);
                }
            }
        }
        PlayerMaterials = materials.ToArray();

        audio = GetComponent<AudioSource>();
        steps.Add(footstep1);
        steps.Add(footstep2);
        steps.Add(footstep3);
        steps.Add(footstep4);
        steps.Add(footstep5);
        steps.Add(footstep6);

        SetJointsActive(false);

        if (img != null) img.gameObject.SetActive(false);
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
                if (Cam.transform.eulerAngles.x < 300 && Cam.transform.eulerAngles.x > 85 && pitch > ang.x) pitch = ang.x;

                camRot = Quaternion.Euler(new Vector3(pitch, ang.y + (camRotationSpeed * Input.GetAxis(CamHoriz) * Time.deltaTime), ang.z));
                lastCamMovement.Restart();
                CamIsDrifting = false;



            }else if ((!lastCamMovement.IsRunning || lastCamMovement.ElapsedMilliseconds > CamCooldown) && !CamIsDrifting && CamDriftEnabled)
            {
                CamIsDrifting = true;
                StartCoroutine(DriftCamToCardinalDirection());
            }

            if ((State == PlayerState.IDLE || State == PlayerState.WALKING || State == PlayerState.LIGHT_ATTACKING) && (Input.GetButtonDown(LightAttackButton) || Input.GetAxis(Trigger) > 0.2) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > LightCooldown))
            {
                PlayerAnimator.SetTrigger("Swing");
                lastAttack.Restart();
                State = PlayerState.LIGHT_ATTACKING;
                //Body.velocity = new Vector3(0, 0, 0); //turn off (or make coroutine) for skid

                StartCoroutine(TargetNearestEnemy());
                Invoke("EnsureAttackComplete",1f);
            }

            if ((State == PlayerState.IDLE || State == PlayerState.WALKING) && (Input.GetButtonDown(HeavyAttackButton)) && (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > HeavyCooldown))
            {
                PlayerAnimator.SetTrigger("Heavy");
                lastAttack.Restart();
                State = PlayerState.HEAVY_ATTACKING;
                //Body.velocity = new Vector3(0, 0, 0); //turn off (or make coroutine) for skid
                audio.clip = heavyswing;
                audio.Play();
                Invoke("EnsureAttackComplete", 1f);

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

            if ((!lastHeal.IsRunning || lastHeal.ElapsedMilliseconds > InteractCooldown) && Input.GetButton(HealButton) && State != PlayerState.DEATH && State != PlayerState.HURT )
            {
                lastHeal.Restart();
                StartCoroutine(UsePotion());
            }

            if(isFlickering)
            {
                if( (Time.frameCount % FlickerSpeed) < (FlickerSpeed / 2))
                {
                    foreach (Material m in PlayerMaterials)
                    {
                        m.color = new Color(m.color.g, m.color.g, m.color.b, FlickerAlpha);
                    }
                }
                else
                {
                    foreach (Material m in PlayerMaterials)
                    {
                        m.color = new Color(m.color.g, m.color.g, m.color.b, 1f);
                    }
                }
            }

            if(State == PlayerState.DEATH && (Input.GetButtonDown(LightAttackButton) || Input.GetButtonDown(HeavyAttackButton)) && (!lastInteract.IsRunning || lastInteract.ElapsedMilliseconds > InteractCooldown))
            {
                StartCoroutine(LoadFromCheckpoint());
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
                        StartCoroutine(Dash(moveDirection * inputForce));
                    }
                    else
                    {
                        State = PlayerState.WALKING;
                        PlayerAnimator.SetBool("IsWalking", true);

                        //Move the player in the direction of the control stick relative to the camera
                        //TODO: Evaluate whether player should be moved via forces, or just have its velocity modified directly.
                        Body.AddForce(moveDirection * inputForce * WalkForce /* * Time.deltaTime*/);

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

                        if (Mathf.Abs(angFrom.y - angTo.y) < rotationSpeed/* * inputForce.magnitude*/)
                        {
                            Body.MoveRotation(Quaternion.Euler(new Vector3(angTo.x, angTo.y, angTo.z)));
                        }
                        else Body.MoveRotation(Quaternion.Euler(new Vector3(angFrom.x, angFrom.y + (sign * rotationSpeed * inputForce.magnitude), angFrom.z)));
                    }
                }
                else
                {
                    State = PlayerState.IDLE;
                    PlayerAnimator.SetBool("IsWalking", false);
                }

                //max speed: the lazy way
                //note that this does not apply in non-movement states (e.g. you can go flying if hurt, or go faster if dashing)
                if (State == PlayerState.IDLE || State == PlayerState.WALKING)
                {
                    if(Body.velocity.magnitude > MaxSpeed)
                    {
                        Body.velocity = Body.velocity.normalized * MaxSpeed;
                    }
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
            //This code prevents dashing into objects to send them flying, but is sloppy and may introduce bugs
            //TODO: edit this code so that you can't stop every rb from moving just by dashing into it 
            if (collision.rigidbody != null) collision.rigidbody.velocity = new Vector3(0, 0, 0);
            Body.velocity = new Vector3(0, 0, 0);
        }
    }

    public void EnsureAttackComplete()
    {
        if(State == PlayerState.LIGHT_ATTACKING)
        {
            if(!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > LightCooldown)
            {
                State = PlayerState.IDLE;
            }
        }
        else if (State == PlayerState.HEAVY_ATTACKING)
        {
            if (!lastAttack.IsRunning || lastAttack.ElapsedMilliseconds > HeavyCooldown)
            {
                State = PlayerState.IDLE;
            }
        }
    }

    public IEnumerator DriftCamToCardinalDirection()
    {

        Vector3[] Cardinals = { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };

        foreach (Vector3 Cardinal in Cardinals)
        {
            if (Vector3.Angle(camRot * Vector3.forward, Cardinal) < CamDriftRange)
            {
                Vector3 angFrom = camRot.eulerAngles; //pitch, yaw, roll

                Quaternion CardinalQuaternion = Quaternion.LookRotation(Cardinal, Vector3.up);
                Vector3 angTo = CardinalQuaternion.eulerAngles;

                //break out of loop once camera ang has aligned (or max frames hit)
                for (int i = 0; i < 1000 && CamIsDrifting; i++)
                {
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

                    //Drift camera towards cardinal direction
                    camRot = Quaternion.Euler(new Vector3(angFrom.x, angFrom.y + sign * CamSnapIncrement, angFrom.z));

                    angFrom = camRot.eulerAngles; //pitch, yaw, roll

                    //Snap to cardinal if close enough, and end the loop
                    if (Mathf.Abs(angTo.y - angFrom.y) <= CamSnapIncrement * 4f)
                    {
                        camRot = Quaternion.Euler(new Vector3(angFrom.x, angTo.y, angFrom.z));
                        break;
                    }

                    yield return new WaitForFixedUpdate();
                }

                break;
            }
        }
        CamIsDrifting = false;

        yield return null;
    }

    public IEnumerator Dash(Vector3 Direction)
    {
        State = PlayerState.DASHING;
        PlayerAnimator.SetBool("IsDashing", true);
        Instantiate(dashsound);
        
        Body.velocity = Direction.normalized * DashSpeed;
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

            if (HealthBarSlider != null)
            {
                HealthBarSlider.value = PlayerHealth.CurrentHealth;
            }
            else UnityEngine.Debug.LogWarning("HealthBarSlider not set in player");

            HealParticleSystem.Play();
            yield return new WaitForSeconds(HealParticleSystem.main.startLifetime.constant+1);
            HealParticleSystem.Stop();
        }
        else
        {
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

        foreach (GameObject e in GameObject.FindGameObjectsWithTag("EnemyTargetPoint"))
        {

            Vector3 MoveDirection = e.transform.position - transform.position;

            //Find amount and direction player should rotate to/in
            Vector3 angFrom = Body.rotation.eulerAngles;
            Vector3 angTo = Quaternion.LookRotation(MoveDirection).eulerAngles;

            if (Vector3.Distance(transform.position, e.transform.position) < closestEnemyDistance && (Mathf.Abs(angFrom.y - angTo.y) < Target_Angular_Range))
            {
                closestEnemy = e;
                closestEnemyDistance = Vector3.Distance(transform.position, e.transform.position);
            }
        }

        if (closestEnemy != null && closestEnemyDistance <= Target_Range)
        {
            bool break1 = false;
            bool break2 = false;

            //FOR some loop of fixed size
            for (int i = 0; i < NumFramesTarget; i++)
            {
                Vector3 MoveDirection = closestEnemy.transform.position - transform.position;

                //Find amount and direction player should rotate to/in
                Vector3 angFrom = Body.rotation.eulerAngles;
                Vector3 angTo = Quaternion.LookRotation(MoveDirection).eulerAngles;

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

                if (Mathf.Abs(angFrom.y - angTo.y) < Target_Angular_Range)
                {
                    if (Mathf.Abs(angFrom.y - angTo.y) < TargetRotationSpeed)
                    {
                        Body.MoveRotation(Quaternion.Euler(new Vector3(0, angTo.y, 0)));
                        break1 = true;
                    }
                    else Body.MoveRotation(Quaternion.Euler(new Vector3(0, angFrom.y + (sign * TargetRotationSpeed), 0)));
                }
                else break; //break if not in angular range (regardless of break1, break2)

                //Only move towards enemy if not already next to it
                if (closestEnemyDistance > Target_Min_Range)
                {
                    Vector3 moveToLoc = new Vector3(Body.position.x, Body.position.y, Body.position.z);
                    if (Body.position.x + Target_Move_Increment <= closestEnemy.transform.position.x)
                    {
                        moveToLoc.x += Target_Move_Increment;
                    }
                    else if (Body.position.x - Target_Move_Increment >= closestEnemy.transform.position.x)
                    {
                        moveToLoc.x -= Target_Move_Increment;
                    }

                    if (Body.position.z + Target_Move_Increment <= closestEnemy.transform.position.z)
                    {
                        moveToLoc.z += Target_Move_Increment;
                    }
                    else if (Body.position.z - Target_Move_Increment >= closestEnemy.transform.position.z)
                    {
                        moveToLoc.z -= Target_Move_Increment;
                    }
                    Body.MovePosition(moveToLoc);
                }
                else break2 = true;

                if (break1 && break2) break;

                //YIELD RETURN next fixed frame
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    //called when sword strike against furniture, etc. causes player to bounce back
    public IEnumerator BounceBack(Vector3 knockback)
    {

        if (State == PlayerState.IDLE || State == PlayerState.LIGHT_ATTACKING || State == PlayerState.HEAVY_ATTACKING)
        {
            audio.Stop();
            audio.clip = clangsound;
            audio.Play();
            State = PlayerState.BOUNCE_BACK;
            PlayerAnimator.SetTrigger("Bounce");
            GetComponent<Rigidbody>().AddForce(knockback);

            yield return new WaitForSeconds(.5f);
            if (State == PlayerState.BOUNCE_BACK) State = PlayerState.IDLE;
        }
        yield return null;
    }

    public void OnImmunityEnd()
    {
        isFlickering = false;

        foreach (Material m in PlayerMaterials)
        {
            m.color = new Color(m.color.g, m.color.g, m.color.b, 1f);
            StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Opaque);
        }
    }

    public void OnDamage(float damage)
    {
        if (HealthBarSlider != null)
        {
            HealthBarSlider.value = PlayerHealth.CurrentHealth;
        }
        else UnityEngine.Debug.LogWarning("HealthBarSlider not set in player");

        if (State != PlayerState.DEATH && damage > 0)
        {
            State = PlayerState.HURT;
            PlayerAnimator.SetTrigger("Damage");
            PlayerAnimator.ResetTrigger("Idle");
            Invoke("SetStateIdle", 0.5f);
            Instantiate(damagesound);
        }

        if (PlayerHealth.CurrentHealth > 0)
        {
            isFlickering = true;
            foreach(Material m in PlayerMaterials)
            {
                StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Fade);
            }
        }
    }

    public void OnDeath(float overkill)
    {
        State = PlayerState.DEATH;
        PlayerAnimator.SetTrigger("Death");

        audio.clip = deathsound;
        audio.Play();

        foreach (Attack a in GetComponent<Equipment>().CurrentWeapon.GetComponents<Attack>()) { Destroy(a); }

        //drop sword
        GetComponent<Equipment>().CurrentWeapon.GetComponent<Weapon>().Holder = null;

        //disable animations
        PlayerAnimator.enabled = false;

        //make ragdoll
        Body.constraints = RigidbodyConstraints.None;
        PlayerCollider.enabled = false;
        SetJointsActive(true);

        //un-parent camera
        ReferenceFrame.transform.SetParent(null);

        //clear interact text
        img.gameObject.SetActive(false);

        //you can restart after a few seconds
        lastInteract.Restart();
    }

    private IEnumerator LoadFromCheckpoint()
    {
        Manager.LoadFromCheckpoint();
        yield return null;
    }

    public void SetJointsActive(bool jointsActive)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            if (rb.tag.Equals("PlayerJoint"))
            {
                rb.isKinematic = !jointsActive;
                Collider c = rb.gameObject.GetComponent<Collider>();
                if (c != null) c.enabled = jointsActive;
                JointToggler j = rb.gameObject.GetComponent<JointToggler>();
                if (j != null) j.enabled = jointsActive;
            }

        }
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
        StartCoroutine(TargetNearestEnemy());
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
                img.text = other.GetComponent<TextHolder>().TextData.ToString();
                Destroy(other.gameObject);
                txt.text = "";
            }
        }
    }

    //debug purposes only

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1f);
        float msec = Time.unscaledDeltaTime * 1000.0f;
        float fps = 1.0f / Time.unscaledDeltaTime;
        string text = "";
        text = string.Concat(text,"\n",string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps));
        Vector3 cam = camRot.eulerAngles;
        text = string.Concat(text,"\n",string.Format("Camera Rotation: {0:0.0}, {1:0.0}, {2:0.0}", cam.x, cam.y, cam.z));
        text = string.Concat(text, "\n", string.Format("Movement input: {0:0.00}, {1:0.00}", Input.GetAxis(MoveHoriz), Input.GetAxis(MoveVert)));
        Vector3 inputForce = new Vector3(Input.GetAxis(MoveHoriz), 0, Input.GetAxis(MoveVert));
        Quaternion moveDirection = Quaternion.Euler(0, camRot.eulerAngles.y, 0);
        inputForce =  moveDirection * inputForce;
        Vector3 camDir = moveDirection * Vector3.forward;
        text = string.Concat(text, "\n", string.Format("Camera Direction: {0:0.000}, {1:0.000}, {2:0.000}", camDir.x, camDir.y, camDir.z));
        text = string.Concat(text, "\n", string.Format("Move Direction: {0:0.000}, {1:0.000}, {2:0.000}", inputForce.x, inputForce.y, inputForce.z));
        Vector3 v = Body.velocity;
        text = string.Concat(text, "\n", string.Format("Player Velocity: {0:0.000}, {1:0.000}, {2:0.000}", v.x, v.y, v.z));
        text = string.Concat(text, "\n", string.Format("\nPlayer State: {0}", State.ToString()));
        GUI.Label(rect, text, style);
    }
    
}

