using UnityEngine;

[System.Serializable]
public class ProgressiveTask : NightTask
{
    [Header("Progresso")]
    public int currentProgress;
    public int targetProgress = 5;

    public override void CheckProgress()
    {
        if (isCompleted) return;

        if (currentProgress >= targetProgress)
            isCompleted = true;
    }

    public void AddProgress(int amount = 1)
    {
        if (isCompleted) return;

        currentProgress += amount;
        CheckProgress();
    }
}
