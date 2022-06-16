using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string ItemType;
    private Inventory inventory;
    private SoundManager soundManager;

    void Start()
    {
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && transform.position == collider.transform.position)
        {
            HandlePickup();
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void HandlePickup()
    {
        inventory.AddItem(ItemType);
        soundManager.PlaySound("pickupItem");
    }
}
