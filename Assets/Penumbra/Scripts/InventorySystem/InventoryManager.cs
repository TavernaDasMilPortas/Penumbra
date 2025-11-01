using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> inventory = new List<InventorySlot>();
    public int maxSlots = 20;

    private Item NullItem => UIInventory.Instance.NullItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Garante slots preenchidos com nullItem no in�cio
        while (inventory.Count < maxSlots)
        {
            inventory.Add(new InventorySlot(UIInventory.Instance.NullItem, 0));
        }
    }

    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null || item.itemName == "null")
        {
            Debug.LogWarning("Tentando adicionar item nulo. Opera��o ignorada.");
            return;
        }

        // Empilha se j� existir
        foreach (InventorySlot slot in inventory)
        {
            if (slot.item == item)
            {
                slot.quantity += quantity;
                UIInventory.Instance.UpdateUI();
                AdditemMessage(item,quantity);
                return;
            }
        }

        // Encontra slot vazio (nullItem)
        foreach (InventorySlot slot in inventory)
        {
            if (slot.item == NullItem)
            {
                slot.item = item;
                slot.quantity = quantity;
                UIInventory.Instance.UpdateUI();
                AdditemMessage(item, quantity);
                return;
            }
        }

        Debug.Log("Invent�rio cheio!");
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || item.itemName == "null")
        {
            Debug.LogWarning("Tentando remover item nulo. Opera��o ignorada.");
            return;
        }

        foreach (InventorySlot slot in inventory)
        {
            if (slot.item == item)
            {
                slot.quantity -= quantity;
                if (slot.quantity <= 0)
                {
                    slot.item = NullItem;
                    slot.quantity = 0;
                }

                UIInventory.Instance.UpdateUI();
                return;
            }
        }
    }

    public void AdditemMessage(Item item, int quantity)
    {
        string formatedMessage = "";
        if (quantity == 1)
        {
            formatedMessage = "+ " + quantity.ToString() + " " + item.itemName;
        }
        else if (quantity > 1)
        {
            formatedMessage = "+ " + quantity.ToString() + " " + item.itemName + "s";
        }
    }
    public bool HasItem(Item item)
    {
        return HasItem(item, 1);
    }


    public bool HasItem(Item item, int quantity)
    {
        if (item == null || item.itemName == "null" || quantity <= 0)
        {
            return false;
        }

        foreach (InventorySlot slot in inventory)
        {
            if (slot.item == item && slot.quantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }
}
