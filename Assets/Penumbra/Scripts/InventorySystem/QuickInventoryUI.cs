using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuickSlotUI
{
    public GameObject slotObject;
    public Image icon;
}

public class QuickInventoryUI : MonoBehaviour
{
    [Header("Referências HUD")]
    public Transform slotContainer;
    public GameObject slotPrefab;
    public Image selectedHighlight;
    public TMP_Text selectedItemName;

    private List<QuickSlotUI> slotUIs = new List<QuickSlotUI>();
    private Coroutine nameFadeRoutine;

    private void OnEnable()
    {
        if (QuickInventoryManager.Instance != null)
            QuickInventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (QuickInventoryManager.Instance != null)
            QuickInventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    private void Start()
    {
        Debug.Log("[QuickInventoryUI] Iniciado.");
        RefreshUI();
    }

    private void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            QuickInventoryManager.Instance.SelectPrevious();
        else if (scroll < 0f)
            QuickInventoryManager.Instance.SelectNext();
    }

    /// <summary>
    /// Atualiza a UI de acordo com o inventário interno.
    /// </summary>
    public void RefreshUI()
    {
        if (QuickInventoryManager.Instance == null)
        {
            Debug.LogError("[QuickInventoryUI] QuickInventoryManager não encontrado!");
            return;
        }

        var inventory = QuickInventoryManager.Instance.internalInventory;
        Debug.Log($"[QuickInventoryUI] Atualizando UI. {inventory.Count} slots no inventário.");

        // 🔹 Remove slots que não existem mais
        while (slotUIs.Count > inventory.Count)
        {
            int lastIndex = slotUIs.Count - 1;
            Destroy(slotUIs[lastIndex].slotObject);
            slotUIs.RemoveAt(lastIndex);
        }

        // 🔹 Cria novos slots se necessário
        while (slotUIs.Count < inventory.Count)
        {
            var slotData = inventory[slotUIs.Count];
            if (slotData.item == null) continue;

            if (slotPrefab == null)
            {
                Debug.LogError("[QuickInventoryUI] SlotPrefab não atribuído!");
                return;
            }

            GameObject obj = Instantiate(slotPrefab, slotContainer);
            Image icon = obj.transform.Find("Icon")?.GetComponent<Image>();

            if (icon == null)
            {
                Debug.LogError("[QuickInventoryUI] Prefab de slot precisa de um filho 'Icon' com componente Image!");
                Destroy(obj);
                return;
            }

            slotUIs.Add(new QuickSlotUI
            {
                slotObject = obj,
                icon = icon
            });
        }

        // 🔹 Atualiza ícones existentes
        for (int i = 0; i < slotUIs.Count; i++)
        {
            var slotData = inventory[i];
            var slotUI = slotUIs[i];

            if (slotData.item == null)
            {
                slotUI.icon.enabled = false;
                continue;
            }

            slotUI.icon.enabled = true;
            slotUI.icon.sprite = slotData.item.icon;
        }

        // 🔹 Atualiza destaque
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        var inventory = QuickInventoryManager.Instance.internalInventory;

        if (slotUIs.Count == 0)
        {
            selectedHighlight.enabled = false;
            selectedItemName.text = "";
            return;
        }

        int selectedIndex = QuickInventoryManager.Instance.selectedIndex;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, slotUIs.Count - 1);

        selectedHighlight.enabled = true;
        selectedHighlight.transform.SetParent(slotUIs[selectedIndex].slotObject.transform, false);
        selectedHighlight.transform.SetAsFirstSibling();

        var selectedItem = inventory[selectedIndex].item;
        if (selectedItem != null)
            StartNameFade(selectedItem.itemName);
        else
            selectedItemName.text = "";
    }

    private void StartNameFade(string itemName)
    {
        if (nameFadeRoutine != null)
            StopCoroutine(nameFadeRoutine);

        selectedItemName.text = itemName;
        nameFadeRoutine = StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        float fadeDuration = 1.5f;
        float fadeDelay = 0.3f;

        Color color = selectedItemName.color;
        color.a = 1f;
        selectedItemName.color = color;

        yield return new WaitForSeconds(fadeDelay);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            selectedItemName.color = color;
            yield return null;
        }

        color.a = 0f;
        selectedItemName.color = color;
    }
}
