using UnityEngine;

[System.Serializable]
public class SimpleTask : NightTask
{
    public override void CheckProgress()
    {
        // Simplesmente marca como concluída.
        if (!isCompleted)
            isCompleted = true;
    }
}
