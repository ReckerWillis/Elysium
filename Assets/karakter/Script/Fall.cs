using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Playercontroller.Instance.TakeDamage(1);
            GameManager.Instance.RespawnPlayer();
            
            if (Playercontroller.Instance.Health <= 0)
            {
                Playercontroller.Instance.pState.alive = false;
                /*StartCoroutine(wait());
                UIManager.Instance.ActivateDeathScreen();*/
                Playercontroller.Instance.Respawned();
            }
            
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
    }
}
