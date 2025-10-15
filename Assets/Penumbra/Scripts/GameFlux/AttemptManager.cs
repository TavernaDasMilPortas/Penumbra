using UnityEngine;

public class AttemptManager : MonoBehaviour
{
    [Header("Tentativas")]
    public int MaxAttempts = 3;
    public int CurrentAttempt = 0;

    public void AddAttempt()
    {
        CurrentAttempt++;
        NightManager.Instance.UpdateUI();
    }

    public void ResetAttempts()
    {
        CurrentAttempt = 0;
        NightManager.Instance.UpdateUI();
    }

    public bool HasExceededMaxAttempts()
    {
        return CurrentAttempt >= MaxAttempts;
    }
}
