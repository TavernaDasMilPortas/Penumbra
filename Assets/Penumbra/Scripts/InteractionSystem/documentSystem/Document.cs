using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Document : MonoBehaviour, IInteractable
{
    [Header("Referência ao Documento")]
    public DocumentData documentData;

    [Header("Estado Atual")]
    public int currentPageIndex = 0;
    public bool showingBackSide = false;

    private MeshRenderer meshRenderer;

    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    private DocumentViewer viewer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        ApplyDocumentMaterial();
        viewer = FindObjectOfType<DocumentViewer>();
        if (viewer == null)
            Debug.LogWarning("[Document] Nenhum DocumentViewer encontrado na cena!");
    }

    /// <summary>
    /// Aplica o material base do documento (feito apenas uma vez).
    /// </summary>
    public void ApplyDocumentMaterial()
    {
        if (documentData == null)
        {
            Debug.LogWarning($"[Document] Documento inválido em {name}");
            return;
        }

        Material mat = documentData.GetMaterial();
        if (mat != null)
            meshRenderer.material = mat;
    }

    /// <summary>
    /// Retorna o texto da página atual (frente ou verso).
    /// </summary>
    public string GetCurrentText()
    {
        return documentData?.GetPageText(currentPageIndex, showingBackSide) ?? string.Empty;
    }

    public void Interact()
    {
        if (RequiredItem == null)
        {
            StartReading();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    /// <summary>
    /// Inicia a leitura do documento, mudando o game state e abrindo o viewer.
    /// </summary>
    public void StartReading()
    {
        if (viewer == null)
        {
            Debug.LogWarning("[Document] DocumentViewer não configurado.");
            return;
        }

        currentPageIndex = 0;
        showingBackSide = false;
        viewer.OpenDocument(this);
        GameStateManager.Instance.SetState(InputState.Document);
        ActionHintManager.Instance.ShowHint("E", "Próxima Página");
        ActionHintManager.Instance.ShowHint("Q", "Página Anterior");
        ActionHintManager.Instance.ShowHint("Esc", "Fechar Documento");
    }



    /// <summary>
    /// Avança para a próxima página ou mostra verso se aplicável.
    /// </summary>
    public void ContinueReading()
    {
        if (documentData == null || viewer == null) return;

        var page = documentData.pages[currentPageIndex];

        // TwoSide: se ainda não mostramos o verso, mostrar agora
        if (page.sideMode == DocumentPage.PageSideMode.TwoSide && !showingBackSide)
        {
            showingBackSide = true;
            viewer.UpdatePage(this);
            return;
        }

        // Próxima página
        if (currentPageIndex < documentData.pages.Count - 1)
        {
            currentPageIndex++;
            showingBackSide = false; // sempre começar a nova página pelo lado da frente
            viewer.UpdatePage(this);
        }
        else
        {
            // Última página → fechar documento
            CloseReading();

        }
    }

    public void PreviousReading()
    {
        if (documentData == null || viewer == null) return;

        var page = documentData.pages[currentPageIndex];

        // Se a página atual for TwoSide e estamos no verso, volta para frente
        if (page.sideMode == DocumentPage.PageSideMode.TwoSide && showingBackSide)
        {
            showingBackSide = false;
            viewer.UpdatePage(this);
            return;
        }

        // Voltar para página anterior
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            var prevPage = documentData.pages[currentPageIndex];

            // Se a página anterior for TwoSide, começar mostrando o verso (ao voltar)
            showingBackSide = prevPage.sideMode == DocumentPage.PageSideMode.TwoSide ? true : false;
            viewer.UpdatePage(this);
        }
    }


    /// <summary>
    /// Fecha o documento e retorna o game state para gameplay.
    /// </summary>
    public void CloseReading()
    {
        if (viewer == null) return;

        viewer.CloseDocument();
        GameStateManager.Instance.RestorePreviousState();
        ActionHintManager.Instance.ClearHints();
    }
}
