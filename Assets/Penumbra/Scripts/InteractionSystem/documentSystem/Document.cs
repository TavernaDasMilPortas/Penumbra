using UnityEngine;

public class Document : InteractableBase
{
    [Header("Referência ao Documento")]
    [SerializeField] public DocumentData documentData;

    [Header("Estado Atual")]
    [SerializeField] private int currentPageIndex = 0;
    [SerializeField] private bool showingBackSide = false;

    private DocumentViewer viewer;

    public string GetCurrentText()
    {
        return documentData?.GetPageText(currentPageIndex, showingBackSide) ?? string.Empty;
    }
    private void Start()
    {
        viewer = DocumentViewer.Instance;
        if (viewer == null)
        {
            viewer = FindObjectOfType<DocumentViewer>();
        }

        Debug.Log($"[DOC] Start() name={name} viewer={(viewer != null ? viewer.name : "NULL")} activeInHierarchy={gameObject.activeInHierarchy} layer={LayerMask.LayerToName(gameObject.layer)}");
        InteractionMessage = "Pressione E para ler o documento";
    }

    public override void Interact()
    {
        Debug.Log($"[DOC] Interact() called on {name} - IsInteractable={IsInteractable}");

        if (!IsInteractable)
        {
            Debug.LogWarning($"[Document] {gameObject.name} não está interagível no momento.");
            return;
        }

        if (RequiredItem != null && !QuickInventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {
            ActionHintManager.Instance.ShowHint("E", $"Necessita: {RequiredItem.itemName} x{RequiredItemQuantity}");
            Debug.Log($"[Document] Item necessário: {RequiredItem.itemName}");
            return;
        }

        StartReading();
    }

    private void StartReading()
    {
        if (viewer == null) return;

        currentPageIndex = 0;
        showingBackSide = false;

        viewer.OpenDocument(this);
        GameStateManager.Instance.SetState(InputState.Document);

        ActionHintManager.Instance.ClearHints();
        ActionHintManager.Instance.ShowHint("E", "Próxima Página");
        ActionHintManager.Instance.ShowHint("Q", "Página Anterior");
        ActionHintManager.Instance.ShowHint("R", "Fechar Documento");
    }

    public void ContinueReading()
    {
        if (documentData == null || viewer == null) return;

        var page = documentData.pages[currentPageIndex];

        if (page.sideMode == DocumentPage.PageSideMode.TwoSide && !showingBackSide)
        {
            showingBackSide = true;
            viewer.UpdatePage(this);
            return;
        }

        if (currentPageIndex < documentData.pages.Count - 1)
        {
            currentPageIndex++;
            showingBackSide = false;
            viewer.UpdatePage(this);
        }
        else
        {
            CloseReading();
        }
    }

    public void PreviousReading()
    {
        if (documentData == null || viewer == null) return;

        var page = documentData.pages[currentPageIndex];

        if (page.sideMode == DocumentPage.PageSideMode.TwoSide && showingBackSide)
        {
            showingBackSide = false;
            viewer.UpdatePage(this);
            return;
        }

        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            var prevPage = documentData.pages[currentPageIndex];
            showingBackSide = prevPage.sideMode == DocumentPage.PageSideMode.TwoSide;
            viewer.UpdatePage(this);
        }
    }

    public void CloseReading()
    {
        if (viewer == null) return;

        viewer.CloseDocument();
        GameStateManager.Instance.RestorePreviousState();
        ActionHintManager.Instance.ClearHints();
        InteractionMessage = "Pressione E para ler o documento";
    }
}
