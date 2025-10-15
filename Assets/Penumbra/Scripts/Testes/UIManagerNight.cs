using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIManagerNight : MonoBehaviour
{
    [Header("Referências UI")]
    public TextMeshProUGUI nightText;
    public TextMeshProUGUI attemptText;
    public TextMeshProUGUI tasksText; // novo campo para listar as tasks

    /// <summary>
    /// Atualiza o número da noite.
    /// </summary>
    public void UpdateNightText(int nightNumber)
    {
        if (nightText)
            nightText.text = $"NOITE: {nightNumber}";
    }

    /// <summary>
    /// Atualiza o contador de tentativas.
    /// </summary>
    public void UpdateAttemptText(int current, int max)
    {
        if (attemptText)
            attemptText.text = $"TENTATIVA: {current}/{max}";
    }

    /// <summary>
    /// Atualiza a lista de tasks na UI.
    /// </summary>
    public void UpdateTasks(List<NightTask> activeTasks)
    {
        if (tasksText == null) return;

        tasksText.text = ""; // limpa antes de preencher
        foreach (var task in activeTasks)
        {
            string status = task.isCompleted ? "V" : "X";
            tasksText.text += $"{status} {task.taskName}\n";
        }
    }
}