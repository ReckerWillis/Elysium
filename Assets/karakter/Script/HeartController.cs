using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    Playercontroller player;

    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartParent;
    public GameObject heartContainerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        player = Playercontroller.Instance;
        heartContainers = new GameObject[Playercontroller.Instance.maxHealth];
        heartFills = new Image[Playercontroller.Instance.maxHealth];

        Playercontroller.Instance.onHealthChangedCallBack += UpdateHeartHUD;
        InstantiateHeartContainer();
        UpdateHeartHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < Playercontroller.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    void SetFilledHeart()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < Playercontroller.Instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainer()
    {
        for (int i = 0;i < Playercontroller.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    void UpdateHeartHUD()
    {
        SetHeartContainers();
        SetFilledHeart();
    }




}
