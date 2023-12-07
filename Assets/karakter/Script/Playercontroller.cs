using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Playercontroller : MonoBehaviour
{
    [Header("Horizontal Movement Setting")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Setting")]
    [SerializeField] private float jumpForce = 45;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [Space(5)]

    [Header("Wall Jump Setting")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]



    [Header("Ground Check Setting")]

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Setting")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [Space(5)]

    [Header("Attack Setting")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health setting")]
    public int health;
    public int maxHealth;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Mana setting")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [Space(5)]

    [Header("Skill Cast setting")]
    //stat
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float skillDamage;
    //objek
    [SerializeField] GameObject sideSkill;
    [SerializeField] GameObject upSkill;
    [Space(5)]

  
    [HideInInspector]public PlayerStateList pState;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float xAxis,yAxis;
    private float gravity;
    Animator anim;
    private bool canDash = true;
    private bool dashed;

    

    public static Playercontroller Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        gravity = rb.gravityScale;

        
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
    }

    //hitbox
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }*/


    // Update is called once per frame
    void Update()
    {
        if (pState.cutscene) return;
        if(pState.alive)
        {
            GetInputs();
        }
        Debug.Log("Position: " + transform.position);
        Debug.Log("Grounded: " + Grounded());
        
        UpdateJumpVariables();
        RestoreTimeScale();

        if (pState.dashing) return;
        if (pState.alive)
        {
            if(!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }
            
            WallSlide();
            WallJump();
            StartDash();
            Attack();
            Heal();
            CastSkill();
        }
        FlashWhileInvincible();
    }
    
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting) 
        {
            _other.GetComponent<Enemy>().EnemyHit(skillDamage,(_other.transform.position - transform.position).normalized,-recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing) return;
        Recoil();
        Debug.Log("Velocity: " + rb.velocity);

    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1.5f,transform.localScale.y);
            pState.lookingRight = false;
        }
        else if(xAxis > 0)
        {
            transform.localScale = new Vector2(1.5f, transform.localScale.y);
            pState.lookingRight = true;
        }
        

        
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;

        }
        if (Grounded())
        {
            dashed = false;
        }
        
    }


    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir ,float _delay)
    {
        //jikakeluardariatas
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 2 : -2;

            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }



    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX,recoilXSpeed);
                /*Instantiate(slashEffect, SideAttackTransform);*/
            }
            else if(yAxis > 0) 
            {
                Hit(UpAttackTransform, UpAttackArea,  ref pState.recoilingY,recoilYSpeed);
                SlashEffectAtAngle(slashEffect,90 ,UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position,_attackArea,0,attackableLayer);

        if(objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit
                    (damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransForm)
    {
        _slashEffect = Instantiate(_slashEffect,_attackTransForm);
        _slashEffect.transform.eulerAngles = new Vector3 (0,0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2 (transform.localScale.x,transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }
        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }

    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        if(pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if(Health <= 0)
            {
                health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }  
    }

    IEnumerator StopTakingDamage()
    {
        pState.invicible = true;
        anim.SetTrigger("TakeDamage");

        yield return new WaitForSeconds(1f);
        pState.invicible = false;
    }

    void FlashWhileInvincible()
    {
        sr.material.color =  pState.invicible ? 
            Color.Lerp (Color.white,Color.black,Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            anim.Play("MainCharIdle");
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }
    }

    void CastSkill()
    {
        if (Input.GetButtonDown("Casting") && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
    }
    IEnumerator CastCoroutine()
    {
        anim.SetBool("Castskill", true);
        yield return new WaitForSeconds(0.15f);

        //side
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _SlashSkill = Instantiate(sideSkill, SideAttackTransform.position, Quaternion.identity);
            //flip skill
            if (pState.lookingRight)
            {
                _SlashSkill.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _SlashSkill.transform.eulerAngles = new Vector2(_SlashSkill.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
        }
        //skill atas
        else if (yAxis > 0)
        {
            Instantiate(upSkill, transform);
            rb.velocity = Vector2.zero;
        }
        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("Castskill", false);
        pState.casting = false;
    }


    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX,0,0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Jump()
    {  
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        anim.SetBool("Jumping", !Grounded());


    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

     private bool Walled()
    {
        return Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);
    }

    void WallSlide()
    {
        if(Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            anim.SetBool("WallSlide",true);

        }
        else
        {
            isWallSliding = false;
            anim.SetBool("WallSlide",false);
        }
    }

    void WallJump()
    {
        if(isWallSliding)
        {
            isWallSliding = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;
            CancelInvoke(nameof(StopWallJumping));
            
        }

        if(Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCounter = 0;
            anim.SetBool("WallJump", true);

            if ((pState.lookingRight && transform.eulerAngles.y == 0) ||(!pState.lookingRight && transform.eulerAngles.y != 0))
            {
                pState.lookingRight = !pState.lookingRight;
                int _yRotation = pState.lookingRight ? 0 : 180;

                transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            }
            
            Invoke(nameof(StopWallJumping),wallJumpingDuration);
        }
    }

    void StopWallJumping()
    {
        isWallJumping = false;
        anim.SetBool("WallJump", false);
    }



}
