using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickSlot
{
    public Item item;
    public int quantity;

    // referências físicas reais coletadas (cada elemento é um GameObject do mundo)
    public List<GameObject> instances = new List<GameObject>();
}

public class QuickInventoryManager : MonoBehaviour
{
    public static QuickInventoryManager Instance;

    [Header("Inventário Dinâmico")]
    public List<QuickSlot> internalInventory = new List<QuickSlot>();
    public int selectedIndex = 0;

    // Evento disparado sempre que o inventário muda
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        internalInventory.Clear();
    }

    /// <summary>Adiciona item ao inventário; se 'instance' for fornecido, guarda referência física.</summary>
    public void AddItem(Item item, int quantity = 1, GameObject instance = null)
    {
        if (item == null) return;

        // procura slot existente
        QuickSlot slot = internalInventory.Find(s => s.item == item);
        if (slot != null)
        {
            slot.quantity += quantity;
            if (instance != null)
            {
                // garante que a instância esteja configurada como ItemInstance
                var instComp = instance.GetComponent<ItemInstance>();
                if (instComp == null) instComp = instance.AddComponent<ItemInstance>();
                instComp.data = item;
                slot.instances.Add(instance);
            }

            OnInventoryChanged?.Invoke();
            return;
        }

        // cria um novo slot
        var newSlot = new QuickSlot
        {
            item = item,
            quantity = quantity,
            instances = new List<GameObject>()
        };

        if (instance != null)
        {
            var instComp = instance.GetComponent<ItemInstance>();
            if (instComp == null) instComp = instance.AddComponent<ItemInstance>();
            instComp.data = item;
            newSlot.instances.Add(instance);
        }

        // --- ADICIONA AO INVENTÁRIO ---
        bool wasEmpty = internalInventory.Count == 0;

        internalInventory.Add(newSlot);

        // dispara UI
        OnInventoryChanged?.Invoke();

        // --- EQUIPA AUTOMATICAMENTE SE ERA O PRIMEIRO ITEM ---
        if (wasEmpty && ArmsManager.Instance != null)
        {
            selectedIndex = 0;

            // garantir que existe instância física
            if (newSlot.instances.Count > 0)
            {
                var inst = newSlot.instances[0];
                inst.SetActive(true); // <- OBRIGATÓRIO
            }

            ArmsManager.Instance.EquipItem(item);
        }

    }

    /// <summary>Remove quantidade do inventário (não remove instâncias físicas a menos que slot esvazie).</summary>
    public void RemoveItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < internalInventory.Count; i++)
        {
            var slot = internalInventory[i];
            if (slot.item == item)
            {
                bool wasEquipped = (i == selectedIndex);

                slot.quantity -= quantity;

                if (slot.quantity <= 0)
                {
                    // Se havia instâncias físicas, elas permanecem (normalmente gerenciadas por holder),
                    // mas removemos o slot vazio.
                    internalInventory.RemoveAt(i);

                    if (selectedIndex >= internalInventory.Count)
                        selectedIndex = Mathf.Max(0, internalInventory.Count - 1);
                }

                OnInventoryChanged?.Invoke();

                if (internalInventory.Count == 0)
                {
                    ArmsManager.Instance?.EquipItem(null);
                }
                else if (wasEquipped)
                {
                    ArmsManager.Instance?.EquipItem(GetSelectedItem());
                }

                return;
            }
        }
    }

    /// <summary>Retorna o ItemScriptable selecionado.</summary>
    public Item GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= internalInventory.Count) return null;
        return internalInventory[selectedIndex].item;
    }

    /// <summary>Retorna true se o jogador tem o item com quantidade mínima.</summary>
    public bool HasItem(Item item, int quantity = 1)
    {
        if (item == null) return false;
        foreach (var slot in internalInventory)
            if (slot.item == item && slot.quantity >= quantity) return true;
        return false;
    }

    /// <summary>Retorna (sem remover) a instância física a ser usada quando equipar o item selecionado.</summary>
    public GameObject GetSelectedInstance()
    {
        var item = GetSelectedItem();
        if (item == null) return null;

        var slot = internalInventory.Find(s => s.item == item);
        if (slot == null || slot.instances.Count == 0) return null;

        // regra D: escolhemos a última adicionada (LIFO) por padrão
        return slot.instances[slot.instances.Count - 1];
    }

    /// <summary>Remove e retorna uma instância física do slot selecionado (usado ao colocar em holder).</summary>
    public GameObject RemoveInstanceFromSelectedSlot()
    {
        var item = GetSelectedItem();
        if (item == null) return null;

        var slot = internalInventory.Find(s => s.item == item);
        if (slot == null || slot.instances.Count == 0) return null;

        // pega a última instância
        var inst = slot.instances[slot.instances.Count - 1];
        slot.instances.RemoveAt(slot.instances.Count - 1);

        // decrementa quantidade também
        slot.quantity = Mathf.Max(0, slot.quantity - 1);
        if (slot.quantity == 0)
        {
            internalInventory.Remove(slot);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, internalInventory.Count - 1));
        }

        OnInventoryChanged?.Invoke();
        return inst;
    }

    /// <summary>Adiciona de volta uma instância física a um slot correspondente ao Item.</summary>
    public void ReturnInstanceToSlot(Item item, GameObject instance)
    {
        if (item == null || instance == null) return;

        var slot = internalInventory.Find(s => s.item == item);
        if (slot == null)
        {
            slot = new QuickSlot { item = item, quantity = 1, instances = new List<GameObject>() { instance } };
            internalInventory.Add(slot);
        }
        else
        {
            slot.quantity += 1;
            slot.instances.Add(instance);
        }

        OnInventoryChanged?.Invoke();
    }

    // seleção por rolagem
    public void SelectNext()
    {
        if (internalInventory.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % internalInventory.Count;
        OnInventoryChanged?.Invoke();
        ArmsManager.Instance?.EquipItem(GetSelectedItem());
    }

    public void SelectPrevious()
    {
        if (internalInventory.Count == 0) return;
        selectedIndex = (selectedIndex - 1 + internalInventory.Count) % internalInventory.Count;
        OnInventoryChanged?.Invoke();
        ArmsManager.Instance?.EquipItem(GetSelectedItem());
    }
}
