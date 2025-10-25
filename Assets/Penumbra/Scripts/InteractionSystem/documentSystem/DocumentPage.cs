using UnityEngine;

[System.Serializable]
public class DocumentPage
{
    public enum PageSideMode
    {
        OneSide,
        TwoSide
    }

    [Header("Configura��o da P�gina")]
    public string pageName = "P�gina";
    public PageSideMode sideMode = PageSideMode.OneSide;

    [Header("Conte�do")]
    [TextArea(5, 15)] public string frontText;

    [Tooltip("Usado apenas se o modo for TwoSide.")]
    [TextArea(5, 15)] public string backText;

    /// <summary>
    /// Retorna o texto de acordo com o lado solicitado.
    /// </summary>
    public string GetText(bool back = false)
    {
        return back && sideMode == PageSideMode.TwoSide ? backText : frontText;
    }
}
