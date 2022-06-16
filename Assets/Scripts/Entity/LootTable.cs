using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    public GameObject LootItem;
    public int PercentageDropChance;
    private SoundManager soundManager;

    void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    public void DropItem()
    {
        // Make sure LootItem is set
        if (LootItem)
        {
            if (Random.Range(1,101) <= PercentageDropChance)
            {
                GameObject item = GameObject.Instantiate(LootItem, transform.position, Quaternion.identity);
                soundManager.PlaySound("lootDrop");
            }
        }
    }
}
