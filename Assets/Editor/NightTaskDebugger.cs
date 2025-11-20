using UnityEngine;
using UnityEditor;

public class NightTaskDebugger : EditorWindow
{
    [MenuItem("Tools/Night System/Task Debugger")]
    public static void OpenWindow()
    {
        GetWindow<NightTaskDebugger>("Night Task Debugger");
    }

    private Vector2 scroll;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🌙 Night Task Debugger", EditorStyles.boldLabel);

        NightManager nm = NightManager.Instance;

        if (nm == null)
        {
            EditorGUILayout.HelpBox("NightManager não está na cena!", MessageType.Warning);
            return;
        }

        TaskManager taskManager = nm.taskManager;

        if (taskManager == null || taskManager.activeTasks == null)
        {
            EditorGUILayout.HelpBox("TaskManager não possui tasks carregadas.", MessageType.Info);
            return;
        }

        NightData night = nm.CurrentNight;
        EditorGUILayout.LabelField($"Noite Atual: {night.nightName}");
        EditorGUILayout.LabelField($"Tasks ativas: {taskManager.activeTasks.Count}");

        EditorGUILayout.Space();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var task in taskManager.activeTasks)
        {
            if (task == null) continue;

            DrawTaskBlock(task);
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.EndScrollView();

        Repaint(); // Atualiza em tempo real
    }

    private void DrawTaskBlock(NightTask task)
    {
        GUIStyle box = new GUIStyle("box");
        box.padding = new RectOffset(10, 10, 10, 10);

        EditorGUILayout.BeginVertical(box);

        EditorGUILayout.LabelField($"📝 {task.taskName}", EditorStyles.boldLabel);

        EditorGUILayout.LabelField($"Tipo: {task.GetType().Name}");
        EditorGUILayout.LabelField($"Concluída: {task.isCompleted}");

        if (task is ProgressiveTask p)
        {
            EditorGUILayout.LabelField($"Progresso: {p.currentProgress} / {p.targetProgress}");
        }
        else if (task is TimedTask t)
        {
            EditorGUILayout.LabelField($"Tempo: {t.elapsedTime:F1} / {t.timeLimit:F1}");
            EditorGUILayout.LabelField($"Falhou: {t.failed}");
            EditorGUILayout.LabelField($"Progresso: {t.currentProgress} / {t.targetProgress}");
        }

        EditorGUILayout.EndVertical();
    }
}
