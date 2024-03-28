using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour
{
    public int healthUpPoint;

    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player"){
            PlayerController playerController = FindObjectOfType<PlayerController>();
            playerController.HealthUp(healthUpPoint);
            Destroy(gameObject);
        }
    }
}
