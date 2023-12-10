using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilfactor;
    [SerializeField] protected bool isRecoiling = false;

    /*[SerializeField] protected Playercontroller player;*/
    [SerializeField] protected float speed;


    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    protected enum EnemyStates
    {
        //slime
        Slime_Idle,
        Slime_Flip,
        
        //bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,


    }
    protected EnemyStates curentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return curentEnemyState; }
        set
        {
            if (curentEnemyState != value)
            {
                curentEnemyState = value;

                ChangeCurrentAnimation();
            }
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        /*player = Playercontroller.Instance;*/
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        if (isRecoiling )
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }

    }
    public virtual void EnemyHit(float _damageDone,Vector2 _hitDirection,float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling )
        {
            rb.AddForce(-_hitForce * recoilfactor * _hitDirection);
        }


    }
    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !Playercontroller.Instance.pState.invicible && health > 0)
        {
            Attack();
            if (Playercontroller.Instance.pState.alive)
            {
                Playercontroller.Instance.HitStopTime(0, 5, 0.5f);
            }
            
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }


    protected virtual void UpdateEnemyStates()
    {

    }
    protected virtual void ChangeCurrentAnimation()
    {

    }
    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }


    protected virtual void Attack()
    {
        Playercontroller.Instance.TakeDamage(damage);
    }


}
