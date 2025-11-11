[System.Serializable]
public abstract class NightTask
{
    public string taskName;
    public bool isCompleted;

    public abstract void CheckProgress();
}