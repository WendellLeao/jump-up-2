using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Intro,
    Playing,
    LevelCompleted,
    PlayerIsDead
}

public static class GameStateManager
{
    public static GameState gameState;

    static GameStateManager()
    {
        gameState = GameState.Intro;
    }
}
