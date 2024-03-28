using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int powerUpPoint;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            PlayerController playerController = FindObjectOfType<PlayerController>();
            StartCoroutine(playerController.PowerUp(powerUpPoint));
            Destroy(gameObject);
        }
    }
}
