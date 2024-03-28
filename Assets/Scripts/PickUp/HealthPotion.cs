using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int restorePoint;

    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player"){
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            player.RestoreHealth(restorePoint);
            Destroy(gameObject);
        }
    }
}
