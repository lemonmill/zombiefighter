using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnitState))]
public class PlayerCombat : MonoBehaviour, IDamagable<DamageObject>
{
    #region Singletone

    public static PlayerCombat instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion
    
    [Header("Linked Components")]
    public Transform weaponBone; //the bone were weapon will be parented on
    private UnitAnimator animator; //link to the animator component
    private UnitState playerState; //the state of the player
    private Rigidbody rb;

    [Header("Attack Data & Combos")]
    public float hitZRange = 2f; //the z range of attacks
    public float critChance = 5;
    public const float CRIT_DAMAGE_MULTIPLIER = 5;
    private int attackNum = -1; //the current attack combo number
    [Space(5)] public DamageObject[] PunchCombo; //a list of punch attacks
    public DamageObject[] KickCombo; //a list of kick Attacks
    [NonSerialized] public bool JumpKickAllowed;
    public DamageObject JumpKickData; //jump kick Attack
    public DamageObject GroundPunchData; //Ground punch Attack
    public DamageObject GroundKickData; //Ground kick Attack
    [NonSerialized] public bool superPunchAllowed;
    [FormerlySerializedAs("RunningPunch")]
    public DamageObject SuperPunch; //punch attack during the run animation
    [NonSerialized] public bool superKickAllowed;
    [FormerlySerializedAs("RunningKick")]
    public DamageObject SuperKick; //kick attack during the run animation
    private DamageObject lastAttack; //data from the last attack that has taken place

    [Header("Settings")]
    public bool blockAttacksFromBehind = false; //block enemy attacks coming from behind
    public bool comboContinueOnHit = true; //only continue a combo when the previous attack was a hit
    public bool resetComboChainOnChangeCombo; //restart a combo when switching to a different combo chain
    public bool invulnerableDuringJump = false; //check if the player can be hit during a jump
    public float hitRecoveryTime = .4f; //the time it takes to recover from a hit
    public float hitThreshold = .2f; //the time before we can get hit again
    public float hitKnockBackForce = 1.5f; //the knockback force when we get hit
    public float GroundAttackDistance = 1.5f; //the distance from an enemy at which a ground attack can be preformed
    public int knockdownHitCount = 3; //the number of times the player can be hit before being knocked down
    public float KnockdownTimeout = 0; //the time before we stand up after a knockdown
    public float KnockdownUpForce = 5; //the Up force of a knockDown
    public float KnockbackForce = 4; //the horizontal force of a knockDown
    public float KnockdownStandUpTime = .8f; //the time it takes for the stand up animation to finish

    [Header("Audio")] public string knockdownVoiceSFX = "";
    public string hitVoiceSFX = "";
    public string deathVoiceSFX = "";
    public string defenceHitSFX = "";
    public string dropSFX = "";

    [Header("Stats")] public DIRECTION currentDirection; //the current direction
    public GameObject itemInRange; //an item that is currently in interactable range
    private Weapon currentWeapon; //the current weapon the player is holding
    private DIRECTION defendDirection; //the direction while defending
    private bool continuePunchCombo; //true if a punch combo needs to continue
    private bool continueKickCombo; //true if the a kick combo needs to  continue
    private float lastAttackTime = 0; //time of the last attack
    private float lastDefendTime = 0;
    [SerializeField] private bool targetHit; //true if the last hit has hit a target
    private int hitKnockDownCount = 0; //the number of times the player is hit in a row
    private int hitKnockDownResetTime = 2; //the time before the hitknockdown counter resets
    private float LastHitTime = 0; // the last time when we were hit 
    private bool isDead = false; //true if this player has died
    private int EnemyLayer; // the enemy layer
    private int DestroyableObjectLayer; // the destroyable object layer
    private int EnvironmentLayer; //the environment layer
    private LayerMask HitLayerMask; // a list of all hittable objects
    private bool isGrounded;
    private Vector3 fixedVelocity;
    private bool updateVelocity;
    private string lastAttackInput;
    private DIRECTION lastAttackDirection;

    //a list of states when the player can attack
    private List<UNITSTATE> AttackStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK,
        UNITSTATE.RUN,
        UNITSTATE.JUMPING,
        UNITSTATE.PUNCH,
        UNITSTATE.KICK,
        UNITSTATE.DEFEND,
    };

    //list of states where the player can be hit
    private List<UNITSTATE> HitableStates = new List<UNITSTATE>
    {
        UNITSTATE.DEFEND,
        UNITSTATE.HIT,
        UNITSTATE.IDLE,
        UNITSTATE.LAND,
        UNITSTATE.PUNCH,
        UNITSTATE.KICK,
        UNITSTATE.THROW,
        UNITSTATE.WALK,
        UNITSTATE.RUN,
        UNITSTATE.GROUNDKICK,
        UNITSTATE.GROUNDPUNCH,
    };

    //list of states where the player can activate defence
    private List<UNITSTATE> DefendStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.DEFEND,
        UNITSTATE.WALK,
        UNITSTATE.RUN,
    };

    //a list of states where the player can change direction
    private List<UNITSTATE> MovementStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK,
        UNITSTATE.RUN,
        UNITSTATE.JUMPING,
        UNITSTATE.JUMPKICK,
        UNITSTATE.LAND,
        UNITSTATE.DEFEND,
    };

    //---

    private void OnEnable()
    {
        InputManager.onInputEvent += OnInputEvent;
        InputManager.onDirectionInputEvent += OnDirectionInputEvent;
    }

    private void OnDisable()
    {
        InputManager.onInputEvent -= OnInputEvent;
        InputManager.onDirectionInputEvent -= OnDirectionInputEvent;
    }

    //awake
    private void Start()
    {
        animator = GetComponentInChildren<UnitAnimator>();
        playerState = GetComponent<UnitState>();
        rb = GetComponent<Rigidbody>();

        //assign layers and layermasks
        EnemyLayer = LayerMask.NameToLayer("Enemy");
        DestroyableObjectLayer = LayerMask.NameToLayer("DestroyableObject");
        EnvironmentLayer = LayerMask.NameToLayer("Environment");
        HitLayerMask = (1 << EnemyLayer) | (1 << DestroyableObjectLayer);

        //display error messages for missing components
        if (!animator) Debug.LogError("No player animator found inside " + gameObject.name);
        if (!playerState) Debug.LogError("No playerState component found on " + gameObject.name);
        if (!rb) Debug.LogError("No rigidbody component found on " + gameObject.name);

        //set invulnerable during jump
        if (!invulnerableDuringJump)
        {
            HitableStates.Add(UNITSTATE.JUMPING);
            HitableStates.Add(UNITSTATE.JUMPKICK);
        }
    }

    private void Update()
    {
        //the player is colliding with the ground
        if (animator) isGrounded = animator.animator.GetBool("isGrounded");

        //update defence state every frame
        Defend(InputManager.defendKeyDown);

        if (Input.GetKeyDown(KeyCode.L))
        {
            Revive();
        }
    }

    //physics update
    private void FixedUpdate()
    {
        if (updateVelocity)
        {
            rb.velocity = fixedVelocity;
            updateVelocity = false;
        }
    }

    //late Update
    private void LateUpdate()
    {
        //apply any root motion offsets to parent
        if (animator && animator.GetComponent<Animator>().applyRootMotion &&
            animator.transform.localPosition != Vector3.zero)
        {
            Vector3 offset = animator.transform.localPosition;
            animator.transform.localPosition = Vector3.zero;
            transform.position += offset * -(int) currentDirection;
        }
    }

    //set velocity in next fixed update
    private void SetVelocity(Vector3 velocity)
    {
        fixedVelocity = velocity;
        updateVelocity = true;
    }

    //movement input event
    private void OnDirectionInputEvent(Vector2 inputVector, bool doubleTapActive)
    {
        if (!MovementStates.Contains(playerState.currentState)) return;
        int dir = Mathf.RoundToInt(Mathf.Sign((float) -inputVector.x));
        if (Mathf.Abs(inputVector.x) > 0) currentDirection = (DIRECTION) dir;
    }

    #region Combat Input

    //combat input event
    private void OnInputEvent(string action, BUTTONSTATE buttonState)
    {
        /*
        Debug.Log("AttackStates.Contains(playerState.currentState): " + AttackStates.Contains(playerState.currentState));
        Debug.Log("action: " + action);
        Debug.Log("buttonstate: " + buttonState);
        Debug.Log("!isDead: " + !isDead);
        */
        
        if (AttackStates.Contains(playerState.currentState) && !isDead)
        {
            // running punch return
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS &&
                playerState.currentState == UNITSTATE.RUN && isGrounded)
            {
                //.Log("Player tried to do runningPunch");
                return;
            }

            // running kick return
            if (action == "Kick" && buttonState == BUTTONSTATE.PRESS &&
                playerState.currentState == UNITSTATE.RUN && isGrounded)
            {
                //Debug.Log("Player tried to do runningKick");
                return;
            }

            // defend punch
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS &&
                playerState.currentState == UNITSTATE.DEFEND && isGrounded)
            {
                DefendPunch();
                return;
            }

            if ((action == "DefendPunch")&&(superPunchAllowed))
            {
                //Debug.Log("Force DefendPunch");
                
                DefendPunch();
                return;
            }

            //defend kick
            if (action == "Kick" && buttonState == BUTTONSTATE.PRESS &&
                playerState.currentState == UNITSTATE.DEFEND && isGrounded)
            {
                DefendKick();
                return;
            }
            
            if ((action == "DefendKick")&&(superKickAllowed))
            {
                //Debug.Log("Force DefendKick");

                DefendKick();
                return;
            }

            //pick up an item
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS && itemInRange != null && isGrounded &&
                currentWeapon == null)
            {
                interactWithItem();
                return;
            }

            //use an weapon
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS && isGrounded && currentWeapon != null)
            {
                useCurrentWeapon();
                return;
            }

            //ground punch
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS &&
                (playerState.currentState != UNITSTATE.PUNCH && NearbyEnemyDown()) && isGrounded)
            {
                if (GroundPunchData.animTrigger.Length > 0) doAttack(GroundPunchData, UNITSTATE.GROUNDPUNCH, "Punch");
                return;
            }

            //ground kick
            if (action == "Kick" && buttonState == BUTTONSTATE.PRESS &&
                (playerState.currentState != UNITSTATE.KICK && NearbyEnemyDown()) && isGrounded)
            {
                if (GroundKickData.animTrigger.Length > 0) doAttack(GroundKickData, UNITSTATE.GROUNDKICK, "Kick");
                return;
            }

            //reset combo when switching to another combo chain (user setting)
            if (resetComboChainOnChangeCombo && (action != lastAttackInput))
            {
                attackNum = -1;
            }

            //default punch
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS && playerState.currentState != UNITSTATE.PUNCH &&
                playerState.currentState != UNITSTATE.KICK && isGrounded)
            {
                DefaultPunch();
                return;
            }

            //advance the punch combo if "punch" was pressed during a punch attack
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS &&
                (playerState.currentState == UNITSTATE.PUNCH) && !continuePunchCombo && isGrounded)
            {
                if (attackNum < PunchCombo.Length - 1)
                {
                    continuePunchCombo = true;
                    continueKickCombo = false;
                    return;
                }
            }

            //jump punch
            if (action == "Punch" && buttonState == BUTTONSTATE.PRESS && !isGrounded)
            {
                Debug.Log(SavableSettings.instance.currentCharacter + "  " + GlobalGameSettings.Player1CharacterID + "  " + GlobalGameSettings.Player1Prefab);
                Debug.Log("");
                if (JumpKickAllowed && JumpKickData.animTrigger.Length > 0)
                {
                    doAttack(JumpKickData, UNITSTATE.JUMPKICK, "Kick");
                    StartCoroutine(JumpKickInProgress());
                }
                return;
            }

            //jump kick
            if (action == "Kick" && JumpKickAllowed && buttonState == BUTTONSTATE.PRESS && !isGrounded)
            {
                if (JumpKickData.animTrigger.Length > 0)
                {
                    doAttack(JumpKickData, UNITSTATE.JUMPKICK, "Kick");
                    StartCoroutine(JumpKickInProgress());
                }

                return;
            }

            //default kick
            if (action == "Kick" && buttonState == BUTTONSTATE.PRESS && playerState.currentState != UNITSTATE.KICK &&
                playerState.currentState != UNITSTATE.PUNCH && isGrounded)
            {
                DefaultKick();
                return;
            }

            //advance the kick combo if "kick" was pressed during a kick attack
            if (action == "Kick" && buttonState == BUTTONSTATE.PRESS && (playerState.currentState == UNITSTATE.KICK) &&
                !continueKickCombo && isGrounded)
            {
                if (attackNum < KickCombo.Length - 1)
                {
                    continueKickCombo = true;
                    continuePunchCombo = false;
                    return;
                }
            }
        }
    }

    private void DefaultKick()
    {
        if ((Time.time - lastDefendTime < 1f)&&(superKickAllowed))
        {
            DefendKick();
            return;
        }
        
//continue to the next attack if the time is inside the combo window
        bool insideComboWindow = (lastAttack != null &&
                                  (Time.time <
                                   (lastAttackTime + lastAttack.duration + lastAttack.comboResetTime)));
        if (insideComboWindow && !continueKickCombo && (attackNum < KickCombo.Length - 1))
        {
            attackNum += 1;
        }
        else
        {
            attackNum = 0;
        }

        doAttack(KickCombo[attackNum], UNITSTATE.KICK, "Kick");
    }

    private void DefaultPunch()
    {
        if ((Time.time - lastDefendTime < 1f)&&(superPunchAllowed))
        {
            DefendPunch();
            return;
        }
        
        //continue to the next attack if the time is inside the combo window
        bool insideComboWindow = (lastAttack != null &&
                                  (Time.time <
                                   (lastAttackTime + lastAttack.duration + lastAttack.comboResetTime)));
        if (insideComboWindow && !continuePunchCombo && (attackNum < PunchCombo.Length - 1))
        {
            attackNum += 1;
        }
        else
        {
            attackNum = 0;
        }

        if (PunchCombo[attackNum] != null && PunchCombo[attackNum].animTrigger.Length > 0)
            doAttack(PunchCombo[attackNum], UNITSTATE.PUNCH, "Punch");
    }

    private void DefendKick()
    {
        lastDefendTime = -1;
        
        
        if (!superKickAllowed) return;

        Defend(false);

        if (SuperKick.animTrigger.Length > 0) doAttack(SuperKick, UNITSTATE.ATTACK, "Kick");
    }

    private void DefendPunch()
    {
        lastDefendTime = -1;
        
        if (!superPunchAllowed) return;

        Defend(false);

        if (SuperPunch.animTrigger.Length > 0) doAttack(SuperPunch, UNITSTATE.ATTACK, "Punch");
    }

    #endregion

    #region Combat functions

    private void doAttack(DamageObject damageObject, UNITSTATE state, string inputAction)
    {
        animator.SetAnimatorTrigger(damageObject.animTrigger);
        playerState.SetState(state);

        //save attack data
        lastAttack = damageObject;
        lastAttack.inflictor = gameObject;
        lastAttackTime = Time.time;
        lastAttackInput = inputAction;
        lastAttackDirection = currentDirection;

        //turn towards current input direction
        TurnToDir(currentDirection);

        if (isGrounded) SetVelocity(Vector3.zero);
        if (damageObject.forwardForce > 0) animator.AddForce(damageObject.forwardForce);

        if (state == UNITSTATE.JUMPKICK) return;
        Invoke("Ready", damageObject.duration);
    }

    //use the currently equipped weapon
    private void useCurrentWeapon()
    {
        playerState.SetState(UNITSTATE.USEWEAPON);
        TurnToDir(currentDirection);
        SetVelocity(Vector3.zero);

        //save attack data
        lastAttackInput = "WeaponAttack";
        lastAttackTime = Time.time;
        lastAttack = currentWeapon.damageObject;
        lastAttack.inflictor = gameObject;
        lastAttackDirection = currentDirection;

        if (!string.IsNullOrEmpty(currentWeapon.damageObject.animTrigger))
            animator.SetAnimatorTrigger(currentWeapon.damageObject.animTrigger);
        if (!string.IsNullOrEmpty(currentWeapon.useSound)) GlobalAudioPlayer.PlaySFX(currentWeapon.useSound);
        Invoke("Ready", currentWeapon.damageObject.duration);

        //weapon degeneration
        if (currentWeapon.degenerateType == DEGENERATETYPE.DEGENERATEONUSE) currentWeapon.useWeapon();
        if (currentWeapon.degenerateType == DEGENERATETYPE.DEGENERATEONUSE && currentWeapon.timesToUse == 0)
            StartCoroutine(destroyCurrentWeapon(currentWeapon.damageObject.duration));
        if (currentWeapon.degenerateType == DEGENERATETYPE.DEGENERATEONHIT && currentWeapon.timesToUse == 1)
            StartCoroutine(destroyCurrentWeapon(currentWeapon.damageObject.duration));
    }

    //remove the current weapon
    private IEnumerator destroyCurrentWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentWeapon.degenerateType == DEGENERATETYPE.DEGENERATEONUSE)
            GlobalAudioPlayer.PlaySFX(currentWeapon.breakSound);
        Destroy(currentWeapon.playerHandPrefab);
        currentWeapon.BreakWeapon();
        currentWeapon = null;
    }

    //returns the current weapon
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    //jump kick in progress
    private IEnumerator JumpKickInProgress()
    {
        animator.SetAnimatorBool("JumpKickActive", true);

        //a list of enemies that we have hit
        List<GameObject> enemieshit = new List<GameObject>();

        //small delay so the animation has time to play
        yield return new WaitForSeconds(.1f);

        //check for hit
        while (playerState.currentState == UNITSTATE.JUMPKICK)
        {
            //draw a hitbox in front of the character to see which objects it collides with
            Vector3 boxPosition = transform.position + (Vector3.up * lastAttack.collHeight) +
                                  Vector3.right * ((int) currentDirection * lastAttack.collDistance);
            Vector3 boxSize = new Vector3(lastAttack.CollSize / 2, lastAttack.CollSize / 2, hitZRange / 2);
            Collider[] hitColliders = Physics.OverlapBox(boxPosition, boxSize, Quaternion.identity, HitLayerMask);

            //hit an enemy only once by adding it to a list
            foreach (Collider col in hitColliders)
            {
                if (!enemieshit.Contains(col.gameObject))
                {
                    enemieshit.Add(col.gameObject);

                    //hit a damagable object
                    IDamagable<DamageObject> damagableObject =
                        col.GetComponent(typeof(IDamagable<DamageObject>)) as IDamagable<DamageObject>;
                    if (damagableObject != null)
                    {
                        damagableObject.Hit(lastAttack);

                        //camera Shake
                        CamShake camShake = Camera.main.GetComponent<CamShake>();
                        if (camShake != null) camShake.Shake(.1f);
                    }
                }
            }

            yield return null;
        }
    }

    //set defence on/off
    private void Defend(bool defend)
    {
        if (!DefendStates.Contains(playerState.currentState)) return;
        animator.SetAnimatorBool("Defend", defend);
        if (defend)
        {
            lastDefendTime = Time.time;
            
            TurnToDir(currentDirection);
            SetVelocity(Vector3.zero);
            playerState.SetState(UNITSTATE.DEFEND);
            animator.SetAnimatorBool("Run", false); //disable running
        }
        else
        {
            if (playerState.currentState == UNITSTATE.DEFEND) playerState.SetState(UNITSTATE.IDLE);
        }
    }

    #endregion

    #region Check For Hit

    //check if we have hit something (Animation Event)
    public void CheckForHit()
    {
        //draw a hitbox in front of the character to see which objects it collides with
        Vector3 boxPosition = transform.position + (Vector3.up * lastAttack.collHeight) +
                              Vector3.right * ((int) lastAttackDirection * lastAttack.collDistance);
        Vector3 boxSize = new Vector3(lastAttack.CollSize / 2, lastAttack.CollSize / 2, hitZRange / 2);
        Collider[] hitColliders = Physics.OverlapBox(boxPosition, boxSize, Quaternion.identity, HitLayerMask);

        //Critical multiplying
        bool isCritical = Random.Range(0, 100) < critChance;
        if (isCritical)
        {
            lastAttack.isCritical = true;
            lastAttack.damage *= CRIT_DAMAGE_MULTIPLIER;
        }
        int i = 0;
        while (i < hitColliders.Length)
        {
            //hit a damagable object
            IDamagable<DamageObject> damagableObject =
                hitColliders[i].GetComponent(typeof(IDamagable<DamageObject>)) as IDamagable<DamageObject>;
            if (damagableObject != null)
            {
                damagableObject.Hit(lastAttack);

                //we have hit something
                targetHit = true;
            }

            i++;
        }

        //Critical cleaning
        if (isCritical)
        {
            isCritical = false;
            lastAttack.damage /= CRIT_DAMAGE_MULTIPLIER;
        }

        //nothing was hit
        if (hitColliders.Length == 0) targetHit = false;

        //on weapon hit
        if (lastAttackInput == "WeaponAttack" && targetHit) currentWeapon.onHitSomething();
    }

    //Display hit box in Unity Editor (Debug)
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lastAttack != null && (Time.time - lastAttackTime) < lastAttack.duration)
        {
            Gizmos.color = Color.red;
            Vector3 boxPosition = transform.position + (Vector3.up * lastAttack.collHeight) +
                                  Vector3.right * ((int) lastAttackDirection * lastAttack.collDistance);
            Vector3 boxSize = new Vector3(lastAttack.CollSize, lastAttack.CollSize, hitZRange);
            Gizmos.DrawWireCube(boxPosition, boxSize);
        }
    }
#endif

    #endregion

    #region We Are Hit

    //we are hit
    public void Hit(DamageObject d)
    {
        //check if we can get hit again
        if (Time.time < LastHitTime + hitThreshold) return;

        //check if we are in a hittable state
        if (HitableStates.Contains(playerState.currentState))
        {
            CancelInvoke();

            //camera Shake
            CamShake camShake = Camera.main.GetComponent<CamShake>();
            if (camShake != null) camShake.Shake(.1f);

            //defend incoming attack
            if (playerState.currentState == UNITSTATE.DEFEND && !d.DefenceOverride &&
                (isFacingTarget(d.inflictor) || blockAttacksFromBehind))
            {
                Defend(d);
                return;
            }
            else
            {
                animator.SetAnimatorBool("Defend", false);
            }

            //we are hit
            UpdateHitCounter();
            LastHitTime = Time.time;

            //show hit effect
            animator.ShowHitEffect();

            //substract health
            HealthSystem healthSystem = GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.SubstractHealth(d.damage);
                if (healthSystem.CurrentHp == 0)
                    return;
            }

            //check for knockdown
            if ((hitKnockDownCount >= knockdownHitCount || !IsGrounded() || d.knockDown) &&
                playerState.currentState != UNITSTATE.KNOCKDOWN)
            {
                hitKnockDownCount = 0;
                StopCoroutine("KnockDownSequence");
                StartCoroutine("KnockDownSequence", d.inflictor);
                GlobalAudioPlayer.PlaySFXAtPosition(d.hitSFX, transform.position + Vector3.up);
                GlobalAudioPlayer.PlaySFXAtPosition(knockdownVoiceSFX, transform.position + Vector3.up);
                return;
            }

            //default hit
            int i = Random.Range(1, 3);
            animator.SetAnimatorTrigger("Hit" + i);
            SetVelocity(Vector3.zero);
            playerState.SetState(UNITSTATE.HIT);

            //add a small force from the impact
            if (isFacingTarget(d.inflictor))
            {
                animator.AddForce(-1.5f);
            }
            else
            {
                animator.AddForce(1.5f);
            }

            //SFX
            GlobalAudioPlayer.PlaySFXAtPosition(d.hitSFX, transform.position + Vector3.up);
            GlobalAudioPlayer.PlaySFXAtPosition(hitVoiceSFX, transform.position + Vector3.up);

            Invoke("Ready", hitRecoveryTime);
        }
    }

    //update the hit counter
    private void UpdateHitCounter()
    {
        if (Time.time - LastHitTime < hitKnockDownResetTime)
        {
            hitKnockDownCount += 1;
        }
        else
        {
            hitKnockDownCount = 1;
        }

        LastHitTime = Time.time;
    }

    //defend an incoming attack
    private void Defend(DamageObject d)
    {
        //show defend effect
        animator.ShowDefendEffect();

        //play sfx
        GlobalAudioPlayer.PlaySFXAtPosition(defenceHitSFX, transform.position + Vector3.up);

        //add a small force from the impact
        if (isFacingTarget(d.inflictor))
        {
            animator.AddForce(-hitKnockBackForce);
        }
        else
        {
            animator.AddForce(hitKnockBackForce);
        }
    }

    #endregion

    #region Item interaction

    //item in range
    public void ItemInRange(GameObject item)
    {
        itemInRange = item;
    }

    //item out of range
    public void ItemOutOfRange(GameObject item)
    {
        if (itemInRange == item) itemInRange = null;
    }

    //interact with an item in range
    public void interactWithItem()
    {
        if (itemInRange != null)
        {
            animator.SetAnimatorTrigger("Pickup");
            playerState.SetState(UNITSTATE.PICKUPITEM);
            SetVelocity(Vector3.zero);
            Invoke("Ready", .3f);
            Invoke("pickupItem", .2f);
        }
    }

    //pick up item
    private void pickupItem()
    {
        if (itemInRange != null)
            itemInRange.SendMessage("OnPickup", gameObject, SendMessageOptions.DontRequireReceiver);
    }

    //equip current weapon
    public void equipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        currentWeapon.damageObject.inflictor = gameObject;

        //add player hand weapon
        if (weapon.playerHandPrefab != null)
        {
            // Instantiating
            GameObject PlayerWeapon = GameObject.Instantiate(weapon.playerHandPrefab) as GameObject;

            // Attaching to weaponBone
            var weaponTransform = PlayerWeapon.transform;
            weaponTransform.parent = weaponBone;
            if (gameObject.name.Contains("FF")) // Female Character Model
            {
                weaponTransform.localPosition = new Vector3(-.0003f, 0, 0);
                weaponTransform.localRotation = Quaternion.Euler(new Vector3(195f, 90f, 97f));
            }
            else // Standart Character Model
            {
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.identity;
            }

            // Assigning
            currentWeapon.playerHandPrefab = PlayerWeapon;
        }
    }

    #endregion

    #region KnockDown Sequence

    //knockDown sequence
    public IEnumerator KnockDownSequence(GameObject inflictor)
    {
        playerState.SetState(UNITSTATE.KNOCKDOWN);
        animator.StopAllCoroutines();
        yield return new WaitForFixedUpdate();

        //look towards the direction of the incoming attack
        int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;
        currentDirection = (DIRECTION) dir;
        TurnToDir(currentDirection);

        //update playermovement
        var pm = GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.CancelJump();
            pm.SetDirection(currentDirection);
        }

        //add knockback force
        animator.SetAnimatorTrigger("KnockDown_Up");
        while (IsGrounded())
        {
            SetVelocity(new Vector3(KnockbackForce * -dir, KnockdownUpForce, 0));
            yield return new WaitForFixedUpdate();
        }

        //going up...
        while (rb.velocity.y >= 0) yield return new WaitForFixedUpdate();

        //going down
        animator.SetAnimatorTrigger("KnockDown_Down");
        while (!IsGrounded()) yield return new WaitForFixedUpdate();

        //hit ground
        animator.SetAnimatorTrigger("KnockDown_End");
        CamShake camShake = Camera.main.GetComponent<CamShake>();
        if (camShake != null) camShake.Shake(.3f);
        animator.ShowDustEffectLand();

        //sfx
        GlobalAudioPlayer.PlaySFXAtPosition(dropSFX, transform.position);

        //ground slide
        float t = 0;
        float speed = 2;
        Vector3 fromVelocity = rb.velocity;
        while (t < 1)
        {
            SetVelocity(Vector3.Lerp(
                new Vector3(fromVelocity.x, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, fromVelocity.z),
                new Vector3(0, rb.velocity.y, 0), t));
            t += Time.deltaTime * speed;
            yield return null;
        }

        //knockDown Timeout
        SetVelocity(Vector3.zero);
        yield return new WaitForSeconds(KnockdownTimeout);

        //stand up
        animator.SetAnimatorTrigger("StandUp");
        playerState.currentState = UNITSTATE.STANDUP;

        yield return new WaitForSeconds(KnockdownStandUpTime);
        playerState.currentState = UNITSTATE.IDLE;
    }

    #endregion

    //returns true if the closest enemy is in a knockdowngrounded state
    private bool NearbyEnemyDown()
    {
        float distance = GroundAttackDistance;
        GameObject closestEnemy = null;
        foreach (GameObject enemy in EnemyManager.activeEnemies)
        {
            //only check enemies in front of us
            if (isFacingTarget(enemy))
            {
                //find closest enemy
                float dist2enemy = (enemy.transform.position - transform.position).magnitude;
                if (dist2enemy < distance)
                {
                    distance = dist2enemy;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            EnemyAI AI = closestEnemy.GetComponent<EnemyAI>();
            if (AI != null && 
                AI.enemyState == UNITSTATE.KNOCKDOWNGROUNDED ||
                AI.enemyState == UNITSTATE.GROUNDHIT)
            {
                return true;
            }
        }

        return false;
    }

    //the attack is finished and the player is ready for new actions
    public void Ready()
    {
        //only continue a combo when we have hit something
        if (comboContinueOnHit && !targetHit)
        {
            continuePunchCombo = continueKickCombo = false;
            lastAttackTime = 0;
        }

        //continue a punch combo
        if (continuePunchCombo)
        {
            continuePunchCombo = continueKickCombo = false;

            if (attackNum < PunchCombo.Length - 1)
            {
                attackNum += 1;
            }
            else
            {
                attackNum = 0;
            }

            if (PunchCombo[attackNum] != null && PunchCombo[attackNum].animTrigger.Length > 0)
                doAttack(PunchCombo[attackNum], UNITSTATE.PUNCH, "Punch");
            return;
        }

        //continue a kick combo
        if (continueKickCombo)
        {
            continuePunchCombo = continueKickCombo = false;

            if (attackNum < KickCombo.Length - 1)
            {
                attackNum += 1;
            }
            else
            {
                attackNum = 0;
            }

            if (KickCombo[attackNum] != null && KickCombo[attackNum].animTrigger.Length > 0)
                doAttack(KickCombo[attackNum], UNITSTATE.KICK, "Kick");
            return;
        }

        playerState.SetState(UNITSTATE.IDLE);
    }

    //returns true is the player is facing a gameobject
    public bool isFacingTarget(GameObject g)
    {
        return ((g.transform.position.x > transform.position.x && currentDirection == DIRECTION.Left) ||
                (g.transform.position.x < transform.position.x && currentDirection == DIRECTION.Right));
    }

    //returns true if the player is grounded
    public bool IsGrounded()
    {
        CapsuleCollider c = GetComponent<CapsuleCollider>();
        float colliderSize = c.bounds.extents.y;
#if UNITY_EDITOR
        Debug.DrawRay(transform.position + c.center, Vector3.down * colliderSize, Color.red);
#endif
        return Physics.Raycast(transform.position + c.center, Vector3.down, colliderSize + .1f, 1 << EnvironmentLayer);
    }

    //turn towards a direction
    public void TurnToDir(DIRECTION dir)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward * -(int) dir);
    }

    //the player has died (Invoked by SendMessage in HealthSystem)
    private void Death()
    {
        if (isDead) return;
        
        isDead = true;
        StopAllCoroutines();
        animator.StopAllCoroutines();
        CancelInvoke();
        SetVelocity(Vector3.zero);
        GlobalAudioPlayer.PlaySFXAtPosition(deathVoiceSFX, transform.position + Vector3.up);
        animator.SetAnimatorBool("Death", true);
        EnemyManager.DisableAllEnemyAIs();
            
        StartCoroutine(ReStartLevel());
    }

    public void Revive()
    {
        Debug.Log("Reviving Player");
        
        isDead = false;
        StopAllCoroutines();
        animator.StopAllCoroutines();
        CancelInvoke();
        SetVelocity(Vector3.zero);
        animator.SetAnimatorBool("Death", false);
        EnemyManager.EnableAllEnemyAIs();
            
        var health = GetComponent<HealthSystem>();
        health.CurrentHp = health.MaxHp;

        GetComponent<PlayerMovement>().Revive();
        
        UIManager.instance.ShowMenu("HUD");
    }

    //restart this level
    private IEnumerator ReStartLevel()
    {
        yield return new WaitForSeconds(2);
        float fadeoutTime = 1.3f;

        UIManager UI = GameObject.FindObjectOfType<UIManager>();
        if (UI != null)
        {
            //fade out
            UI.UI_fader.Fade(UIFader.FADE.FadeOut, fadeoutTime, 0);
            yield return new WaitForSeconds(fadeoutTime);

            //TODO: Add money after death
            //GlobalGameSettings.shouldGiveReward = true;
            
            //show game over screen
            UI.DisableAllScreens();
            UI.ShowMenu("GameOver");
        }
    }
}