using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UserInterface : MonoBehaviour
{
    [Header("User Interface")]
    [SerializeField] private GameObject introUI;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject levelCompletedUI;
    [SerializeField] private GameObject heartUI;

    [Header("Others")]
    [SerializeField] private GameObject tryAgainObject;
    
    private void FixedUpdate() 
    {
        if(GameStateManager.gameState == GameState.Intro)
        {
            heartUI.SetActive(false);
            introUI.SetActive(true);
            Cursor.visible = true;
        }   

        else if(GameStateManager.gameState == GameState.Playing)
        {
            tryAgainObject.SetActive(false);
            introUI.SetActive(false);
            deathUI.SetActive(false);
            heartUI.SetActive(true);
            Cursor.visible = false;
        } 

        else if(GameStateManager.gameState == GameState.PlayerIsDead)
        {
            StartCoroutine(TimeToReloadScene());
            heartUI.SetActive(false);
            deathUI.SetActive(true);
            Cursor.visible = true;
        } 

        else if(GameStateManager.gameState == GameState.LevelCompleted)
        {
            StartCoroutine(PassTheLevel());
        }
    }

    //IEnumerators
    IEnumerator TimeToReloadScene()
    {
        yield return new WaitForSeconds(0.3f);
        tryAgainObject.SetActive(true);
        SceneManagement.canLoadScene = true;
    }
    IEnumerator PassTheLevel()
    {
        heartUI.SetActive(false);
        yield return new WaitForSeconds(1.8f);
        levelCompletedUI.SetActive(true);
        Cursor.visible = true;
        SceneManagement.canLoadScene = true;
    }
}
