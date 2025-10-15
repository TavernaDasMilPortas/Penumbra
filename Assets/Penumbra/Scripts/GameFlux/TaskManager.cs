using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia as tarefas de uma noite em runtime, sem tocar nos ScriptableObjects.
/// </summary>
public class TaskManager
{
    public List<NightTask> activeTasks = new List<NightTask>();

    /// <summary>
    /// Inicializa as tasks a partir de uma referência do NightData.
    /// </summary>
    public void LoadFromNightData(NightData nightData)
    {
        activeTasks.Clear();
        foreach (var t in nightData.tasks)
        {
            activeTasks.Add(new NightTask
            {
                taskName = t.taskName,
                isCompleted = t.isCompleted // normalmente todas começam false
            });
        }
    }

    /// <summary>
    /// Marca uma task como completa pelo nome.
    /// </summary>
    public bool CompleteTask(string taskName)
    {
        foreach (var t in activeTasks)
        {
            if (t.taskName == taskName)
            {
                if (!t.isCompleted)
                {
                    t.isCompleted = true;
                    Debug.Log($"✅ Task '{taskName}' completada em runtime.");
                    return true;
                }
                return false; // já completada
            }
        }

        Debug.LogWarning($"⚠️ Task '{taskName}' não encontrada em runtime.");
        return false;
    }

    /// <summary>
    /// Verifica se todas as tasks estão completas.
    /// </summary>
    public bool AreAllTasksComplete()
    {
        foreach (var t in activeTasks)
            if (!t.isCompleted) return false;
        return true;
    }

    /// <summary>
    /// Reseta todas as tasks em runtime.
    /// </summary>
    public void ResetTasks()
    {
        foreach (var t in activeTasks)
            t.isCompleted = false;
    }
}
