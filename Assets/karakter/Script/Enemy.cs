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

    protected enum EnemyStates
    {
        Slime_Idle,
        Slime_Flip
    }
    protected EnemyStates curentEnemyState;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        /*player = Playercontroller.Instance;*/
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();

        if(health <= 0)
        {
            Destroy(gameObject);
        }
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
        if (_other.gameObject.CompareTag("Player") && !Playercontroller.Instance.pState.invicible)
        {
            Attack();
            Playercontroller.Instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void UpdateEnemyStates()
    {

    }
    protected void ChangeState(EnemyStates _newState)
    {
        curentEnemyState = _newState;
    }


    protected virtual void Attack()
    {
        Playercontroller.Instance.TakeDamage(damage);
    }


}
