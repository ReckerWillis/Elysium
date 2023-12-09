using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartShards : MonoBehaviour
{
    public Image fill;

    public float targetFillAmount;
    public float lerpDuration = 1.5f;
    public float initialFillAmount;


    public IEnumerator LerpFill()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime/lerpDuration);

            float lerpedFillaAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
            fill.fillAmount = lerpedFillaAmount;

            yield return null;

        }

        fill.fillAmount = targetFillAmount;

        if(fill.fillAmount == 1)
        {
            Playercontroller.Instance.maxHealth++;
            Playercontroller.Instance.onHealthChangedCallBack();
            Playercontroller.Instance.heartShards = 0;
        }
    }
}
