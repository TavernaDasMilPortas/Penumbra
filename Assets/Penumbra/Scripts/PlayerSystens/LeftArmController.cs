using UnityEngine;

public class LeftArmController : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public Transform itemSocket; // posição na mão
    private GameObject currentItemObject;
    private Item currentItemData;

    public void SetEquippedItem(Item newItem)
    {
        if (newItem == currentItemData) return; // evita recriar o mesmo

        // 🔹 Limpa item anterior
        if (currentItemObject != null)
            Destroy(currentItemObject);

        currentItemData = newItem;

        // 🔹 Instancia o prefab do novo item, se houver
        if (newItem != null && newItem.handPrefab != null)
        {
            currentItemObject = Instantiate(newItem.handPrefab, itemSocket);
        }

        // 🔹 Atualiza o estado de animação correspondente
        //if (animator != null)
        //{
        //    animator.Play(newItem != null && !string.IsNullOrEmpty(newItem.leftArmState)
        //        ? newItem.leftArmState
        //        : "Empty");
        //}

        Debug.Log($"[LeftArm] Equipado item: {newItem?.itemName ?? "nenhum"}");
    }
}
