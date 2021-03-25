using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Cam : MonoBehaviour
{
    private void FixedUpdate() 
    {
        if(GameStateManager.gameState == GameState.PlayerIsDead || (GameStateManager.gameState == GameState.LevelCompleted))
        {
            GetComponent<CinemachineVirtualCamera>().Follow = null;
        }
    }
}
