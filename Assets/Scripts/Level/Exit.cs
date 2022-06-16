using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private TurnSystem turnSystem;

    void Start()
    {
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && transform.position == collider.transform.position)
        {
            transform.GetComponent<Collider2D>().enabled = false;

            // If this is last scene -> player wins
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                turnSystem.PlayerWin();
            }
            // If this isn't last scene -> load next level
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }            
        }
    }
}
