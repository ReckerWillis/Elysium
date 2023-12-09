using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour
{
    [SerializeField] GameObject canvasUI;
    [SerializeField] HeartShards heartShards;

    bool used;
    // Start is called before the first frame update
    void Start()
    {
        if (Playercontroller.Instance.maxHealth >= Playercontroller.Instance.maxTotalHealth)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());

        }
    }
    IEnumerator ShowUI()
    {
        /*yield return new WaitForSeconds(0.5f);*/
        canvasUI.SetActive(true);

        heartShards.initialFillAmount = Playercontroller.Instance.heartShards * 0.25f;
        Playercontroller.Instance.heartShards++;
        heartShards.targetFillAmount = Playercontroller.Instance.heartShards * 0.25f;

        StartCoroutine(heartShards.LerpFill());

        yield return new WaitForSeconds(2.5f);
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
