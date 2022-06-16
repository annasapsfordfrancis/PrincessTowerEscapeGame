using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void ReturnToMenuScreen()
    {
        SceneManager.LoadScene(0);
    }
}
