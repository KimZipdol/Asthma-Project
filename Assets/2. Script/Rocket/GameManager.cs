using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FIRST_SCREEN,
    TUTORIAL,
    PLAYING,
    ROCKET_LAUNCH,
    GAME_RESULT,
    RANKING
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }


    private void Start()
    {
        
    }
}
