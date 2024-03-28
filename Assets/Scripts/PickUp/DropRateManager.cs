using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    public List<Drops> drops = new List<Drops>();

    public void EnemyDrops()
    {
        if(drops != null){
            float rand = Random.Range(0f, 100f);
            List<Drops> possibleDrops = new List<Drops>();

            foreach (var rate in drops){
                if(rand <= rate.dropRate){
                    possibleDrops.Add(rate);
                }
            }

            if(possibleDrops.Count > 0){
                var randomDrops = Random.Range(0, possibleDrops.Count);
                Drops drops = possibleDrops[randomDrops];
                Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
            }   
        }
    }
}

[System.Serializable]
public class Drops
{
    public string itemName;
    public GameObject itemPrefab;
    [Range(0f, 100f)] public float dropRate;
}
