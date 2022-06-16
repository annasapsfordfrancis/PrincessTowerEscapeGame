using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private TurnSystem turnSystem;
    void Start()
    {
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
    }

    public void TogglePause()
    {

        if (TurnSystem.state == TurnState.PLAYERTURN || TurnSystem.state == TurnState.ENEMYTURN)
        {
            turnSystem.PauseGame();
        }
    }
}
