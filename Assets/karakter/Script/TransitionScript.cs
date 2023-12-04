using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
    {
        if (transitionTo == GameManager.Instance.transitionedFromScene)
        {
            Playercontroller.Instance.transform.position = startPoint.position;

            StartCoroutine(Playercontroller.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }



    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            Playercontroller.Instance.pState.cutscene = true;

            /*SceneManager.LoadScene(transitionTo);*/
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }

}
