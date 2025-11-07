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
    public RectTransform slotContainer;
    public GameObject slotPrefab;
    public Image selectedHighlight;
    public TMP_Text selectedItemName;

    private List<QuickSlotUI> slotUIs = new List<QuickSlotUI>();
    private Coroutine nameFadeRoutine;
    private Coroutine alignRoutine;

    private Canvas rootCanvas;
    private RectTransform canvasRT;

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
        rootCanvas = slotContainer.GetComponentInParent<Canvas>();
        if (rootCanvas == null)
        {
            Debug.LogError("[QuickInventoryUI] Nenhum Canvas encontrado!");
            return;
        }

        canvasRT = rootCanvas.transform as RectTransform;

        if (selectedHighlight != null)
        {
            selectedHighlight.transform.SetParent(rootCanvas.transform, false);
            selectedHighlight.transform.SetAsLastSibling();
        }

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

    // =====================================================
    // Atualiza completamente a UI do inventário rápido
    // =====================================================
    public void RefreshUI()
    {
        if (QuickInventoryManager.Instance == null) return;

        var inventory = QuickInventoryManager.Instance.internalInventory;

        // 🔹 Se o inventário estiver vazio, desativa highlight e limpa texto
        if (inventory.Count == 0)
        {
            if (selectedHighlight != null)
                selectedHighlight.enabled = false;

            if (selectedItemName != null)
                selectedItemName.text = "";

            // Remove slots antigos
            foreach (var s in slotUIs)
                if (s?.slotObject != null)
                    Destroy(s.slotObject);

            slotUIs.Clear();
            return;
        }

        // 🔹 Remove slots excedentes
        while (slotUIs.Count > inventory.Count)
        {
            int last = slotUIs.Count - 1;
            if (slotUIs[last]?.slotObject != null)
                Destroy(slotUIs[last].slotObject);
            slotUIs.RemoveAt(last);
        }

        // 🔹 Cria novos slots
        while (slotUIs.Count < inventory.Count)
        {
            if (slotPrefab == null) return;
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            Image icon = obj.transform.Find("Icon")?.GetComponent<Image>();

            slotUIs.Add(new QuickSlotUI { slotObject = obj, icon = icon });
        }

        // 🔹 Atualiza ícones
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

        // 🔹 Atualiza destaque com leve delay
        if (alignRoutine != null) StopCoroutine(alignRoutine);
        alignRoutine = StartCoroutine(DelayedHighlightUpdate());
    }

    private IEnumerator DelayedHighlightUpdate()
    {
        yield return new WaitForEndOfFrame();
        UpdateHighlight(true);
    }

    // =====================================================
    // Atualiza posição e nome do item selecionado
    // =====================================================
    private void UpdateHighlight(bool instant = false)
    {
        if (selectedHighlight == null) return;

        var inventory = QuickInventoryManager.Instance.internalInventory;

        // 🔹 Se não houver itens, desativa o highlight e limpa o nome
        if (inventory == null || inventory.Count == 0 || slotUIs.Count == 0)
        {
            selectedHighlight.enabled = false;
            if (selectedItemName != null)
                selectedItemName.text = "";
            return;
        }

        int selectedIndex = Mathf.Clamp(QuickInventoryManager.Instance.selectedIndex, 0, slotUIs.Count - 1);
        var slotUI = slotUIs[selectedIndex];

        if (slotUI?.slotObject == null)
        {
            selectedHighlight.enabled = false;
            return;
        }

        RectTransform iconRT = slotUI.icon.GetComponent<RectTransform>();
        RectTransform highlightRT = selectedHighlight.rectTransform;

        Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        // Centraliza o highlight no ícone
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, iconRT.TransformPoint(iconRT.rect.center));
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screenPoint, cam, out localPoint))
        {
            highlightRT.anchoredPosition = localPoint;
        }

        // Não mexe no tamanho, escala ou rotação — deixa manual no editor
        selectedHighlight.enabled = true;
        selectedHighlight.transform.SetAsLastSibling();

        // Atualiza nome do item
        var selectedItem = inventory[selectedIndex].item;
        if (selectedItem != null)
            StartNameFade(selectedItem.itemName);
        else
            selectedItemName.text = "";
    }

    // =====================================================
    // Fade suave do nome do item selecionado
    // =====================================================
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
