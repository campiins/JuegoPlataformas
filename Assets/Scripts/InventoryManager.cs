using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Inventory Manager is NULL");

            return instance;
        }
    }

    [SerializeField] private List<GameObject> inventory = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(GameObject item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(GameObject item)
    {
        inventory.Remove(item);
    }

    public bool HasItem(GameObject item)
    {
        return inventory.Contains(item);
    }
}
