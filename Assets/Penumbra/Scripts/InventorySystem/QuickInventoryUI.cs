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

    public static QuickInventoryUI Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnEnable()
    {
        // 🧩 Se inscreve no evento
        if (QuickInventoryManager.Instance != null)
            QuickInventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        // ❌ Remove inscrição para evitar erros
        if (QuickInventoryManager.Instance != null)
            QuickInventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    private void Start()
    {
        Debug.Log("[QuickInventoryUI] Start chamado");
        RefreshUI();
    }

    private void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) QuickInventoryManager.Instance.SelectPrevious();
        else if (scroll < 0f) QuickInventoryManager.Instance.SelectNext();
    }

    public void RefreshUI()
    {
        Debug.Log("[QuickInventoryUI] RefreshUI chamado");

        var inventory = QuickInventoryManager.Instance.internalInventory;
        Debug.Log($"[QuickInventoryUI] Inventário contém {inventory.Count} slots");

        // Limpa slots antigos
        foreach (var s in slotUIs)
            Destroy(s.slotObject);
        slotUIs.Clear();

        // Cria novos slots
        for (int i = 0; i < inventory.Count; i++)
        {
            var slot = inventory[i];
            if (slot.item == null || slot.quantity <= 0) continue;

            GameObject obj = Instantiate(slotPrefab, slotContainer);
            Image icon = obj.transform.Find("Icon")?.GetComponent<Image>();

            if (icon == null)
            {
                Debug.LogError("[QuickInventoryUI] Prefab precisa de um filho chamado 'Icon' com um Image!");
                continue;
            }

            icon.sprite = slot.item.icon;
            icon.enabled = true;

            slotUIs.Add(new QuickSlotUI
            {
                slotObject = obj,
                icon = icon
            });

            Debug.Log($"[QuickInventoryUI] Criado slot UI para item {slot.item.itemName}");
        }

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
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

        var selectedItem = QuickInventoryManager.Instance.internalInventory[selectedIndex].item;
        if (selectedItem != null)
            StartNameFade(selectedItem.itemName);
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
