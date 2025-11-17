using UnityEngine;

public class DogFoodBowlInteractable : MonoBehaviour, IInteractable
{
    [Header("Itens Necessários")]
    public Item dogFoodItem;
    public Item canOpenerItem;

    [Header("Prefab/Objeto da Ração no Pote")]
    public GameObject foodInBowlObject;

    [Header("Mensagem de interação")]
    [TextArea] public string interactionMessage = "Colocar comida no pote";

    [Header("Flag")]
    public bool isEmpyt = true;

    private bool isInteractable = true;
    public bool IsInteractable => isInteractable;

    public Item RequiredItem => null; // Interação especial -> não usa RequiredItem padrão
    public int RequiredItemQuantity => 0;
    public string InteractionMessage => interactionMessage;

    public void Interact()
    {
        if (!isInteractable)
            return;

        var inv = QuickInventoryManager.Instance;

        bool hasFood = inv.HasItem(dogFoodItem, 1);
        bool hasOpener = inv.HasItem(canOpenerItem, 1);

        if (!hasFood || !hasOpener)
        {
            Debug.Log("Você precisa da comida de cachorro e do abridor.");
            return;
        }

        // ===============================
        // REMOVE ITENS DO INVENTÁRIO
        // ===============================
        inv.RemoveItem(dogFoodItem, 1);
        inv.RemoveItem(canOpenerItem, 1);

        // ===============================
        // ATIVA A RAÇÃO NO POTE
        // ===============================
        if (foodInBowlObject != null)
        {
            foodInBowlObject.SetActive(true);
            isEmpyt = false;
        }

        Debug.Log("Você colocou comida no pote!");

        // Bloqueia interação futura
        isInteractable = false;
    }
}
