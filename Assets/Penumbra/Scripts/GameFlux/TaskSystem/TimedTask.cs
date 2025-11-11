using UnityEngine;

[System.Serializable]
public class TimedTask : NightTask
{
    [Header("Configuração do Tempo")]
    public float timeLimit = 10f; // tempo máximo em segundos
    [HideInInspector] public float elapsedTime = 0f;
    public bool failed;

    // Pode ser opcional, para permitir progresso dentro do tempo
    public int currentProgress;
    public int targetProgress = 1;

    public override void CheckProgress()
    {
        if (failed) return;

        if (currentProgress >= targetProgress)
            isCompleted = true;
    }

    public void AddProgress(int amount = 1)
    {
        if (failed || isCompleted) return;

        currentProgress += amount;
        CheckProgress();
    }

    public void UpdateTimer(float deltaTime)
    {
        if (isCompleted || failed) return;

        elapsedTime += deltaTime;

        if (elapsedTime >= timeLimit)
        {
            failed = true;
            isCompleted = false;
            Debug.Log($"⏰ Tarefa '{taskName}' falhou por tempo!");
        }
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        failed = false;
    }
}
