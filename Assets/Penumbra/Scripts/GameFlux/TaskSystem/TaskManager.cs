using System.Collections.Generic;
using UnityEngine;

public class TaskManager
{
    public List<NightTask> activeTasks = new List<NightTask>();

    public void LoadFromNightData(NightData data)
    {
        activeTasks.Clear();

        if (data.tasks != null)
        {
            foreach (var task in data.tasks)
            {
                // Faz uma cópia independente do ScriptableObject
                string json = JsonUtility.ToJson(task);
                NightTask clone = null;

                if (task is SimpleTask)
                    clone = JsonUtility.FromJson<SimpleTask>(json);
                else if (task is ProgressiveTask)
                    clone = JsonUtility.FromJson<ProgressiveTask>(json);
                else if (task is TimedTask)
                    clone = JsonUtility.FromJson<TimedTask>(json);

                if (clone != null)
                    activeTasks.Add(clone);
            }
        }
    }

    public bool CompleteTask(string taskName)
    {
        foreach (var task in activeTasks)
        {
            if (task.taskName == taskName)
            {
                task.CheckProgress();
                return true;
            }
        }
        return false;
    }

    public void AddProgress(string taskName, int amount)
    {
        foreach (var task in activeTasks)
        {
            if (task.taskName == taskName)
            {
                if (task is ProgressiveTask p)
                    p.AddProgress(amount);
                else if (task is TimedTask t)
                    t.AddProgress(amount);
                break;
            }
        }
    }

    public void UpdateTimedTasks(float deltaTime)
    {
        foreach (var task in activeTasks)
        {
            if (task is TimedTask timed)
                timed.UpdateTimer(deltaTime);
        }
    }

    public bool AreAllTasksComplete()
    {
        foreach (var task in activeTasks)
        {
            if (!task.isCompleted)
                return false;
        }
        return true;
    }
}
