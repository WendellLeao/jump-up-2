using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
   [Header("Variavbles")]
   public static SceneManagement instance;
   public static bool canLoadScene = false;
   
   //Unity Functions
   private void Awake() 
   {
      MakeSingleton();
   }
   private void Update() 
   {
      if(GameStateManager.gameState == GameState.Intro)
        {
            if(Input.GetKeyDown(KeyCode.Space)) LoadCurrentScene();
        }
        
      else if (GameStateManager.gameState == GameState.PlayerIsDead)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canLoadScene)
            {
                ReloadScene();
                canLoadScene = false;
            }
        }

      else if (GameStateManager.gameState == GameState.LevelCompleted)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canLoadScene)
            {
                LoadNextScene();
                canLoadScene = false;
            }
        }
   }

   //SceneManagement Functions
   public void LoadCurrentScene()
   {
      AudioManager.instance.Play("ButtonSound");
      GameStateManager.gameState = GameState.Playing;
   }
   public void ReloadScene()
   {
        PlayerController.lifes = 3;
        AudioManager.instance.Play("ButtonSound");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameStateManager.gameState = GameState.Playing;
   }
   public void LoadNextScene()
   {
         AudioManager.instance.Play("ButtonSound");
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
         GameStateManager.gameState = GameState.Playing;
   }

   void MakeSingleton()
   {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
   }
}
