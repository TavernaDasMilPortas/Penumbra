
using UnityEngine;

public class AutoItemInstaller : MonoBehaviour
{
    [Header("Onde os itens serão instanciados e escondidos")]
    public Transform hiddenItemsParent;

    [Header(" (Opcional) Definir uma layer para esses itens")]
    public LayerMask itemsLayer;

    private void Awake()
    {
        InstallAllItems();
    }

    private void InstallAllItems()
    {
        // Se não existir o parent, cria um automaticamente
        if (hiddenItemsParent == null)
        {
            GameObject container = new GameObject("HiddenItems");
            container.transform.SetParent(transform);
            hiddenItemsParent = container.transform;
        }

        // Busca TODOS os itens existentes no projeto
        Item[] allItems = Resources.FindObjectsOfTypeAll<Item>();

        foreach (var item in allItems)
        {
            if (item == null || item.handPrefab == null)
                continue;

            // Cria um contêiner individual para organizar
            GameObject holder = new GameObject(item.itemName + "_HiddenModel");
            holder.transform.SetParent(hiddenItemsParent);

            // Cria o modelo escondido
            GameObject instantiated = Instantiate(item.handPrefab, holder.transform);

            // Desativa para não aparecer no jogo
            instantiated.SetActive(false);

            // Aplica layer se desejado
            if (itemsLayer.value != 0)
            {
                int layerNumber = Mathf.RoundToInt(Mathf.Log(itemsLayer.value, 2));
                SetLayerRecursively(instantiated, layerNumber);
            }

            Debug.Log($"[AutoItemInstaller] Item preparado: {item.itemName}");
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
