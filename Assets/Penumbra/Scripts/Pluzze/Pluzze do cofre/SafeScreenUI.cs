using TMPro;
using UnityEngine;

public class SafeScreenUI : MonoBehaviour
{
    public TextMeshProUGUI screenText;

    public void UpdateScreen(string text)
    {
        screenText.text = text;
    }
}
