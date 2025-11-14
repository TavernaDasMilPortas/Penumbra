using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    public Transform handSocket;

    private Item currentItem;
    private GameObject currentModel;

    public void EquipItem(Item item)
    {
        // desativa o item anterior
        if (currentModel != null)
            currentModel.SetActive(false);

        currentItem = item;

        // encontra o modelo já existente dentro do socket
        currentModel = FindModelInHand(item.handPrefab.name);

        if (currentModel == null)
        {
            Debug.LogError($"Modelo '{item.handPrefab.name}' não encontrado dentro do Player!");
            return;
        }

        // ativa e posiciona
        currentModel.SetActive(true);
        currentModel.transform.SetParent(handSocket);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localEulerAngles = Vector3.zero;
    }

    public GameObject UnequipItem()
    {
        if (currentModel == null) return null;

        currentModel.SetActive(false);
        return currentModel;
    }

    private GameObject FindModelInHand(string prefabName)
    {
        foreach (Transform t in handSocket)
        {
            if (t.name == prefabName)
                return t.gameObject;
        }
        return null;
    }
}
