using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWallJump : MonoBehaviour
{

    [SerializeField] GameObject canvasUI;
    bool used;
    // Start is called before the first frame update
    void Start()
    {
        if (Playercontroller.Instance.unlockedWallJump)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if(_collision.CompareTag("Player")&& !used)
        {
            used = true;
            StartCoroutine(ShowUI());
            
        }
    }
    IEnumerator ShowUI()
    {
        /*yield return new WaitForSeconds(0.5f);*/
        canvasUI.SetActive(true);

        yield return new WaitForSeconds(4f);
        Playercontroller.Instance.unlockedWallJump = true;
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
