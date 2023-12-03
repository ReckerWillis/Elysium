using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Campfire campfire;

    public static GameManager Instance {get; private set;}

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
        campfire = FindObjectOfType<Campfire>();
    }

    public void RespawnPlayer()
    {   
        if(campfire != null)
        {
            if (campfire.interacted)
            {
                respawnPoint = campfire.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        Playercontroller.Instance.transform.position = respawnPoint;

        StartCoroutine(UIManager.Instance.DeadactivateDeathScreen());
        Playercontroller.Instance.Respawned();
    }

}
