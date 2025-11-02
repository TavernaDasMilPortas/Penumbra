using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickSlot
{
    public Item item;
    public int quantity;
}

public class QuickInventoryManager : MonoBehaviour
{
    public static QuickInventoryManager Instance;

    [Header("Inventário Dinâmico")]
    public List<QuickSlot> internalInventory = new List<QuickSlot>();
    public int selectedIndex = 0;

    // 🔔 Evento disparado sempre que o inventário muda
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        internalInventory.Clear(); // Começa com inventário vazio
    }

    /// <summary>
    /// Adiciona item ao inventário rápido, expandindo se necessário.
    /// </summary>
    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null) return;

        // Tenta empilhar em item existente
        foreach (var slot in internalInventory)
        {
            if (slot.item == item)
            {
                slot.quantity += quantity;
                Debug.Log($"[QuickInventoryManager] Empilhou {item.itemName} x{quantity}");
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        // Nenhum slot com o item encontrado → cria novo slot
        internalInventory.Add(new QuickSlot
        {
            item = item,
            quantity = quantity
        });

        Debug.Log($"[QuickInventoryManager] Criou novo slot: {item.itemName} x{quantity}");
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Remove item do inventário rápido, removendo slot se vazio.
    /// </summary>
    public void RemoveItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < internalInventory.Count; i++)
        {
            var slot = internalInventory[i];
            if (slot.item == item)
            {
                slot.quantity -= quantity;
                Debug.Log($"[QuickInventoryManager] Removeu {item.itemName} x{quantity}");

                if (slot.quantity <= 0)
                {
                    Debug.Log($"[QuickInventoryManager] Removeu slot vazio de {item.itemName}");
                    internalInventory.RemoveAt(i);

                    // Ajusta índice selecionado se necessário
                    if (selectedIndex >= internalInventory.Count)
                        selectedIndex = Mathf.Max(0, internalInventory.Count - 1);
                }

                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    /// <summary>
    /// Retorna item atualmente selecionado.
    /// </summary>
    public Item GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= internalInventory.Count) return null;
        return internalInventory[selectedIndex].item;
    }

    /// <summary>
    /// Verifica se o item selecionado é o item especificado.
    /// </summary>
    public bool IsItemSelected(Item targetItem)
    {
        return GetSelectedItem() == targetItem;
    }

    /// <summary>
    /// Move para o próximo item disponível (rolagem para baixo).
    /// </summary>
    public void SelectNext()
    {
        if (internalInventory.Count == 0) return;

        int oldIndex = selectedIndex;
        selectedIndex = (selectedIndex + 1) % internalInventory.Count;

        Debug.Log($"[QuickInventoryManager] Próximo slot: {oldIndex} ➜ {selectedIndex}");
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Move para o item anterior disponível (rolagem para cima).
    /// </summary>
    public void SelectPrevious()
    {
        if (internalInventory.Count == 0) return;

        int oldIndex = selectedIndex;
        selectedIndex = (selectedIndex - 1 + internalInventory.Count) % internalInventory.Count;

        Debug.Log($"[QuickInventoryManager] Slot anterior: {oldIndex} ➜ {selectedIndex}");
        OnInventoryChanged?.Invoke();
    }
}
