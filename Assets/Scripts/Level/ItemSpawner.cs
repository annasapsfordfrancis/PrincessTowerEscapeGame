using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> ItemList;
    public List<int> ItemWeights;
    public int ChanceToSpawnNothing;
    private List<int> itemRollTable = new List<int>();
    private int maxRoll;
    void Start()
    {
        // Creates a roll table from a list of weights
        for (int i = 0; i < ItemWeights.Count; i++)
        {
            maxRoll += ItemWeights[i];
            itemRollTable.Add(maxRoll);
        }

        maxRoll += ChanceToSpawnNothing;
        int randomRoll = Random.Range(1, maxRoll + 1);
        int itemIndex = GetItemIndex(randomRoll);

        // Only spawn an item if we didn't roll nothing on roll table
        if (itemIndex >= 0)
        {
            GameObject item = GameObject.Instantiate(ItemList[itemIndex], transform.position, Quaternion.identity);
        }
    }

    int GetItemIndex(int randomRoll)
    {
        for (int i = 0; i < itemRollTable.Count; i++)
        {
            if (itemRollTable[i] >= randomRoll)
            {
                return i;
            }
        }
        // We rolled nothing on roll table
        return -1;
    }

    // Draws a square in inspector where enemies will be spawned
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f,1f,0f));
    }
}
