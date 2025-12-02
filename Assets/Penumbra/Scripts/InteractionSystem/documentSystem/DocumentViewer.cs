using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    [Header("Referências de UI")]
    public CanvasGroup viewerCanvas;          // Canvas principal
    public TMP_Text titleText;                // Nome do documento
    public TMP_Text contentText;              // Texto principal
    public ScrollRect scrollRect;             // Scroll do texto
    public Image shadowOverlay;               // Fundo escuro

    [Header("Configuração da exibição 3D")]
    public Transform viewPoint;               // Posição onde o documento ficará ao ser lido
    public float tiltAngle = 15f;             // Inclinação do documento
    public float moveSpeed = 5f;

    private Document currentDocument;
    private Transform documentOriginalParent;
    private Vector3 documentOriginalPosition;
    private Quaternion documentOriginalRotation;
    public Document CurrentDocument => currentDocument;
    private bool isViewing;

    public static DocumentViewer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        viewerCanvas.alpha = 0;
        viewerCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Inicia a leitura de um documento.
    /// </summary>
    public void OpenDocument(Document document)
    {
        if (isViewing) return;
        Debug.Log($"[DOCVIEWER] OpenDocument() called for {document.name} isViewing={isViewing}");
        currentDocument = document;
        documentOriginalParent = document.transform.parent;
        documentOriginalPosition = document.transform.position;
        documentOriginalRotation = document.transform.rotation;

        // Move para o ponto de exibição
        document.transform.SetParent(viewPoint);
        document.transform.localPosition = Vector3.zero;
        document.transform.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        // Atualiza UI
        titleText.text = document.documentData.documentTitle;
        contentText.text = document.GetCurrentText();
        scrollRect.verticalNormalizedPosition = 1;

        // Mostra UI
        viewerCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeCanvas(true));

        isViewing = true;
    }

    /// <summary>
    /// Fecha a leitura e devolve o documento.
    /// </summary>
    public void CloseDocument()
    {
        if (!isViewing || currentDocument == null) return;
        Debug.Log($"[DOCVIEWER] CloseDocument() called isViewing={isViewing}");
        // Retorna o documento à origem
        currentDocument.transform.SetParent(documentOriginalParent);
        currentDocument.transform.position = documentOriginalPosition;
        currentDocument.transform.rotation = documentOriginalRotation;

        StartCoroutine(FadeCanvas(false));
        isViewing = false;
    }

    private System.Collections.IEnumerator FadeCanvas(bool fadeIn)
    {
        float duration = 0.3f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = fadeIn ? (t / duration) : (1 - t / duration);
            viewerCanvas.alpha = alpha;
            shadowOverlay.color = new Color(0, 0, 0, 0.6f * alpha);
            yield return null;
        }

        if (!fadeIn)
            viewerCanvas.gameObject.SetActive(false);
    }
    public bool IsViewing()
    {
        return isViewing;
    }

    public void UpdatePage(Document document)
    {
        contentText.text = document.GetCurrentText();
        scrollRect.verticalNormalizedPosition = 1; // Volta o scroll para o topo
    }

}
