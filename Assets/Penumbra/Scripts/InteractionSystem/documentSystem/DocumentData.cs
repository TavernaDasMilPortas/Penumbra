using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Documents/Document Data")]
public class DocumentData : ScriptableObject
{
    public string documentTitle;

    public List<DocumentPage> pages = new List<DocumentPage>();

    public string GetPageText(int pageIndex, bool backSide = false)
    {
        if (pageIndex < 0 || pageIndex >= pages.Count)
            return string.Empty;

        return pages[pageIndex].GetText(backSide);
    }
}
