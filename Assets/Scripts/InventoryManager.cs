using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    public static InventoryManager Instance { get { return instance; } }

    [SerializeField] private List<GameObject> inventory = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
