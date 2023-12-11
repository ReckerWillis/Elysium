using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DemonBoss : Enemy
{
    public static DemonBoss Instance;
    public Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    public Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] GameObject slashEffect;

    public float attackRange;
    public float attackTimer;

    [HideInInspector]public bool facingRight;

    [Header("Ground Check Setting")]

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    int hitCounter;
    bool stunned, canStun;
    bool alive;

    [HideInInspector] public float runSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.Demon_stage1);
        alive = true;
        
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //hitbox
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if(!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
        if (health <= 0)
        {
            anim.SetTrigger("Die");
            Death(10);
        }
    }

    public void Flip()
    {
        sr.flipX = Playercontroller.Instance.transform.position.x > transform.position.x;
        /*if (Playercontroller.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }*/
    }


    protected override void UpdateEnemyStates()
    {
        if(Playercontroller.Instance != null)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Demon_stage1:
                    break;

                case EnemyStates.Demon_stage2: 
                    break;

                case EnemyStates.Demon_stage3: 
                    break;

                case EnemyStates.Demon_stage4: 
                    break;

            }
        }
    }
    protected override void OnCollisionStay2D(Collision2D _other)
    {

    }

    #region attacking
    #region variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    #endregion

    #region Control

    public void AttackHandler()
    {
        if ( curentEnemyState == EnemyStates.Demon_stage1)
        {
            if(Vector2.Distance(Playercontroller.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                return;
            }

        }
        
    }

    public void ResetAllAttack()
    {
        attacking = false;
        StopCoroutine(TripleSlash());
    }



    #endregion


    #region Stage 1
    IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.8f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(1f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(1f);
        anim.ResetTrigger("Slash");

        ResetAllAttack();
    }

    void SlashAngle()
    {
        if(Playercontroller.Instance.transform.position.x > transform.position.x ||
            Playercontroller.Instance.transform.position.x < transform.position.x)
        {
            Instantiate(slashEffect, SideAttackTransform);
        }
        if (Playercontroller.Instance.transform.position.y > transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        }
        if (Playercontroller.Instance.transform.position.y < transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, -90, UpAttackTransform);
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransForm)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransForm);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
    #endregion

    #endregion

    protected override void Death(float _destroyTime)
    {
        ResetAllAttack();
        alive = false;
        rb.velocity = new Vector2(rb.velocity.x,-25);
        anim.SetTrigger("Die");
    }

    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }


}
