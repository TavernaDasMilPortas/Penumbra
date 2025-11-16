using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickSlot
{
    public Item item;
    public int quantity;
    public List<GameObject> instances = new();
}

public class QuickInventoryManager : MonoBehaviour
{
    public static QuickInventoryManager Instance;

    public List<QuickSlot> internalInventory = new();
    public int selectedIndex = 0;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        internalInventory.Clear();
    }

    // =====================================================================
    // ADD ITEM
    // =====================================================================
    public void AddItem(Item item, int quantity = 1, GameObject instance = null)
    {
        if (item == null) return;

        bool inventoryWasEmpty = internalInventory.Count == 0;

        // 1) Localiza slot
        QuickSlot slot = internalInventory.Find(s => s.item == item);

        // 2) Cria slot ou atualiza
        if (slot == null)
        {
            slot = new QuickSlot { item = item, quantity = quantity };
            internalInventory.Add(slot);
        }
        else
        {
            slot.quantity += quantity;
        }

        // 3) Registra instância física
        if (instance != null)
        {
            var instComp = instance.GetComponent<ItemInstance>();
            if (instComp == null) instComp = instance.AddComponent<ItemInstance>();
            instComp.data = item;

            slot.instances.Add(instance);
        }

        // 4) Dispara atualização de inventário
        OnInventoryChanged?.Invoke();

        // ============================================================
        //            EQUIPAR AUTOMATICAMENTE O PRIMEIRO ITEM
        // ============================================================
        if (inventoryWasEmpty)
        {
            selectedIndex = 0;

            // precisamos esperar 1 frame para garantir que o braço
            // e os transforms já atualizaram depois do OnInventoryChanged
            StartCoroutine(EquipNextFrame(item));
        }
    }

    private System.Collections.IEnumerator EquipNextFrame(Item item)
    {
        // espera 1 frame
        yield return null;

        var instObj = GetSelectedInstance();
        if (instObj != null)
            instObj.SetActive(true);

        ArmsManager.Instance?.EquipItem(item);
    }


    // =====================================================================
    // REMOVE ITEM
    // =====================================================================
    public void RemoveItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < internalInventory.Count; i++)
        {
            var slot = internalInventory[i];
            if (slot.item != item) continue;

            bool wasEquipped = (i == selectedIndex);

            slot.quantity -= quantity;

            if (slot.quantity <= 0)
            {
                internalInventory.RemoveAt(i);
                selectedIndex = Mathf.Clamp(selectedIndex, 0, internalInventory.Count - 1);
            }

            OnInventoryChanged?.Invoke();

            // inventário vazio
            if (internalInventory.Count == 0)
            {
                ArmsManager.Instance?.EquipItem(null);
                return;
            }

            // equipa próximo item
            if (wasEquipped)
            {
                ArmsManager.Instance?.EquipItem(GetSelectedItem());

                var inst = GetSelectedInstance();
                if (inst != null)
                    inst.SetActive(true);
            }

            return;
        }
    }


    // =====================================================================
    public Item GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= internalInventory.Count)
            return null;

        return internalInventory[selectedIndex].item;
    }

    public bool HasItem(Item item, int quantity = 1)
    {
        foreach (var slot in internalInventory)
            if (slot.item == item && slot.quantity >= quantity)
                return true;

        return false;
    }

    // =====================================================================
    public GameObject GetSelectedInstance()
    {
        var slot = (selectedIndex >= 0 && selectedIndex < internalInventory.Count)
            ? internalInventory[selectedIndex]
            : null;

        if (slot == null || slot.instances.Count == 0)
            return null;

        return slot.instances[^1];
    }

    // =====================================================================
    public GameObject RemoveInstanceFromSelectedSlot()
    {
        if (selectedIndex < 0 || selectedIndex >= internalInventory.Count)
            return null;

        var slot = internalInventory[selectedIndex];
        if (slot == null || slot.instances.Count == 0)
            return null;

        // verifica se o item removido era o item atualmente equipado
        bool removedEquippedItem = true;
        // (pois sempre removemos do slot selecionado)

        // pega a instância física
        var inst = slot.instances[^1];
        slot.instances.RemoveAt(slot.instances.Count - 1);

        // reduz quantidade
        slot.quantity--;

        // se esvaziou o slot, remove
        if (slot.quantity <= 0)
        {
            internalInventory.RemoveAt(selectedIndex);

            // ajusta index para o próximo item possível
            selectedIndex = Mathf.Clamp(selectedIndex, 0, internalInventory.Count - 1);
        }

        // avisa UI
        OnInventoryChanged?.Invoke();

        // =============================================================
        // EQUIPAR AUTOMATICAMENTE O PRÓXIMO ITEM SE O ATUAL FOI REMOVIDO
        // =============================================================
        if (removedEquippedItem && internalInventory.Count > 0)
        {
            // equipa o novo item selecionado
            ArmsManager.Instance?.EquipItem(GetSelectedItem());

            // ativa sua instância física
            var newInst = GetSelectedInstance();
            if (newInst != null)
                newInst.SetActive(true);
        }

        return inst;
    }


    // =====================================================================
    public void ReturnInstanceToSlot(Item item, GameObject instance)
    {
        var slot = internalInventory.Find(s => s.item == item);

        if (slot == null)
        {
            slot = new QuickSlot { item = item, quantity = 1 };
            slot.instances.Add(instance);
            internalInventory.Add(slot);
        }
        else
        {
            slot.quantity++;
            slot.instances.Add(instance);
        }

        OnInventoryChanged?.Invoke();
    }

    // =====================================================================
    public void SelectNext()
    {
        if (internalInventory.Count == 0) return;

        selectedIndex = (selectedIndex + 1) % internalInventory.Count;

        var inst = GetSelectedInstance();
        if (inst != null) inst.SetActive(true);

        ArmsManager.Instance?.EquipItem(GetSelectedItem());
        OnInventoryChanged?.Invoke();
    }

    public void SelectPrevious()
    {
        if (internalInventory.Count == 0) return;

        selectedIndex = (selectedIndex - 1 + internalInventory.Count) % internalInventory.Count;

        var inst = GetSelectedInstance();
        if (inst != null) inst.SetActive(true);

        ArmsManager.Instance?.EquipItem(GetSelectedItem());
        OnInventoryChanged?.Invoke();
    }
}
