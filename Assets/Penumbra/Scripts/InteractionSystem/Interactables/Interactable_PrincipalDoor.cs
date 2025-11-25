using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable_PrincipalDoor : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    [Header("Flag")]
    public bool isInteractable = true;
    public bool IsInteractable => isInteractable;

    public void Interact()
    {
        if (!IsInteractable) return;

        // 🔥 SE TODAS AS TASKS ESTÃO COMPLETAS → IR PARA O MENU
        if (QuickInventoryManager.Instance.HasItem(RequiredItem, 1))
        {
            Debug.Log("🎉 Todas as tarefas concluídas — indo para o Main Menu...");
            SceneManager.LoadScene("Main Menu");
            return;
        }

        // 🔥 Caso contrário segue lógica normal do item
        if (RequiredItem == null)
        {
            PerformInteraction();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    private void PerformInteraction()
    {
        Debug.Log(InteractionMessage);
    }
}
