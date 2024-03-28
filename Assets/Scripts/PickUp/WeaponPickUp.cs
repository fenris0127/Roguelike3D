using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public GameObject weaponPrefab;

    public int weaponDamage;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            Transform weaponHolder = GameObject.FindGameObjectWithTag("WeaponHolder").transform;

            foreach(Transform child in weaponHolder){
                Destroy(child.gameObject);
            }

            GameObject newWeapon = Instantiate(weaponPrefab, weaponHolder);
            newWeapon.transform.SetParent(weaponHolder);

            Destroy(gameObject);
        } 
    }
}
