using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    public int speedPoint;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            PlayerController playerController = FindObjectOfType<PlayerController>();
            StartCoroutine(playerController.SpeedUp(speedPoint));
            Destroy(gameObject);
        }
    }
}
