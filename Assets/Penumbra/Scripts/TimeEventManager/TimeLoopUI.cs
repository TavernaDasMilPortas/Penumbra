using UnityEngine;
using UnityEngine.UI; // troque por TMPro se for usar TextMeshPro
using TMPro;
public class TimeLoopUI : MonoBehaviour
{
    [Header("Referências")]
    public TimeLoopManager timeLoopManager;
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (timeLoopManager == null || timerText == null) return;

        float currentTime = timeLoopManager.CurrentTime;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
