using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartObject : MonoBehaviour
{
    private bool gotHeart = false;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.name == "Player"){
            gotHeart = true;
            GetComponent<Animator>().SetTrigger("wasCollected");
            Destroy(this.gameObject, 0.7f);
        }
    }

    private void FixedUpdate() 
    {
        if(gotHeart)
        {
            AudioManager.instance.Play("CollectSound");
            PlayerController.lifes += 1;
            gotHeart = false;
        }    
    }
}
