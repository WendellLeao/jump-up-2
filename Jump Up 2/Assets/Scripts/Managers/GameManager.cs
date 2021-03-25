using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public class GameManager : MonoBehaviour
{
[Header("Variavbles")]
   public static GameManager instance;
   
   //Unity Functions
   private void Awake() 
   {
      MakeSingleton();
   }

    //GameManager Functions
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
