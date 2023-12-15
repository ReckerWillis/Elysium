using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pindahScene : MonoBehaviour
{
    public void pindah (string tujuan){
        SceneManager.LoadScene(tujuan);
    }
    public void QuitButton()
    {
            Application.Quit();
        Debug.Log("Tombol Keluar Telah Ditekan!");
    }
}
