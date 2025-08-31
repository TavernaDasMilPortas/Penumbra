using UnityEngine;
using System;

public class TimeLoopManager : MonoBehaviour
{
    [Header("Configuração do Tempo")]
    public int startTimeInSeconds = 300;

    private float currentTime;
    private bool isRunning = true;
    private int lastSecond;

    public float CurrentTime => currentTime;

    // Eventos
    public static event Action<int> OnSecondPassed;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        int currentSecond = Mathf.FloorToInt(currentTime);

        // dispara quando um novo segundo passa
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            OnSecondPassed?.Invoke(currentSecond);
        }

    }

    public void ResetTimer()
    {
        currentTime = startTimeInSeconds;
        lastSecond = Mathf.FloorToInt(currentTime);
        isRunning = true;
    }
}
