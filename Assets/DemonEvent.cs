using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonEvent : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        if (Playercontroller.Instance.transform.position.x > transform.position.x ||
            Playercontroller.Instance.transform.position.x < transform.position.x)
        {
            Hit(DemonBoss.Instance.SideAttackTransform,DemonBoss.Instance.SideAttackArea);
        }
        else if (Playercontroller.Instance.transform.position.y > transform.position.y)
        {
            Hit(DemonBoss.Instance.UpAttackTransform, DemonBoss.Instance.UpAttackArea);
        }
        else if (Playercontroller.Instance.transform.position.y < transform.position.y)
        {
            Hit(DemonBoss.Instance.DownAttackTransform, DemonBoss.Instance.DownAttackArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for(int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<Playercontroller>() != null)
            {
                _objectsToHit[i].GetComponent<Playercontroller>().TakeDamage(DemonBoss.Instance.damage);
            }
        }
    }

    void DestroyAfterDeath()
    {
        DemonBoss.Instance.DestroyAfterDeath();
    }


}
