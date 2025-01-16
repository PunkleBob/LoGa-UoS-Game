using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public enum State
    {
        None,
        Menu,
        Game,
        Pause,
        GameOver
    }

    private State currentState = State.None;

    public void ChangeState(State newState)
    {
        if (currentState == newState)
        {
            return;
        }

        switch (newState)
        {
            case State.Menu:
                break;
            case State.Game:
                Time.timeScale = 1;
                break;
            case State.Pause:
                Time.timeScale = 0;
                break;
            case State.GameOver:
                break;
            default:
                break;
        }

        currentState = newState;
    }
}
