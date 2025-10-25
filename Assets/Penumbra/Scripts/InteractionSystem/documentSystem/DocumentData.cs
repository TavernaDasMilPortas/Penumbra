using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Documents/Document Data")]
public class DocumentData : ScriptableObject
{
    [Header("Configura��o do Documento")]
    public string documentTitle;

    [Tooltip("Lista de p�ginas deste documento.")]
    public List<DocumentPage> pages = new List<DocumentPage>();

    [Header("Visual do Documento")]
    [Tooltip("Material do documento (define o visual do papel).")]
    public Material documentMaterial;

    /// <summary>
    /// Retorna o texto da p�gina e lado especificados.
    /// </summary>
    public string GetPageText(int pageIndex, bool backSide = false)
    {
        if (pageIndex < 0 || pageIndex >= pages.Count)
            return string.Empty;

        return pages[pageIndex].GetText(backSide);
    }

    /// <summary>
    /// Retorna o material do documento.
    /// </summary>
    public Material GetMaterial()
    {
        return documentMaterial;
    }
}
