using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class Collectable : InteractableBase
{
    [Header("Item a ser Coletado")]
    [SerializeField] private Item collectableItem;
    [SerializeField] public int collectableQuantity = 1;

    private ItemInstance itemInstance;

    public bool isCollected = false;

    // 🔥 Evento disparado ao coletar
    public event Action<Collectable> OnCollected;

    private void Awake()
    {
        itemInstance = GetComponent<ItemInstance>();
        if (itemInstance == null)
            itemInstance = gameObject.AddComponent<ItemInstance>();

        itemInstance.data = collectableItem;
        itemInstance.originalLayer = gameObject.layer;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        QuickInventoryManager.Instance.AddItem(collectableItem, collectableQuantity, gameObject);

        isCollected = true;
        IsInteractable = false;

        // dispara o evento
        OnCollected?.Invoke(this);

        gameObject.SetActive(false);
    }
}
