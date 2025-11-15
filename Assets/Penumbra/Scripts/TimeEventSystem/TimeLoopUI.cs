using UnityEngine;
using TMPro;

public class TimeLoopUI : MonoBehaviour
{
    [Header("Referências")]
    public TimeLoopManager timeLoopManager;
    public TextMeshProUGUI timerText;
    public RightArmController rightArm; // <-- adicionado

    private void Update()
    {
        if (timeLoopManager == null || timerText == null || rightArm == null)
            return;

        // Mostra apenas quando estiver segurando Q
        timerText.enabled = rightArm.IsHoldingUp;

        if (!rightArm.IsHoldingUp)
            return;

        float currentTime = timeLoopManager.CurrentTime;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}