using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DogAI))]
public class DogAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Desenha o inspector padrão
        DrawDefaultInspector();

        DogAI ai = (DogAI)target;
        if (ai == null)
            return;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== DEBUG DO DOG AI ===", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Execute o jogo para ver o estado ao vivo e usar controles de debug.", MessageType.Info);
            return;
        }

        // ============================================================
        // ESTADOS ATUAIS
        // ============================================================
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Estado Atual", ai.CurrentState.ToString());
        EditorGUILayout.LabelField("Agente Pronto", ai.agentReady ? "Sim" : "Não");

        // Segurança contra Null RoomTracker
        if (ai.roomTracker != null)
        {
            string session = ai.roomTracker.CurrentSession != null
                ? ai.roomTracker.CurrentSession.sessionName
                : "Nenhuma";

            string room = ai.roomTracker.CurrentRoom != null
                ? ai.roomTracker.CurrentRoom.roomName
                : "Nenhuma";

            EditorGUILayout.LabelField("Sessão Atual", session);
            EditorGUILayout.LabelField("Sala Atual", room);
        }
        else
        {
            EditorGUILayout.LabelField("RoomTracker", "Não encontrado!");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // ============================================================
        // BOTÕES DE DEBUG (FSM)
        // ============================================================
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Forçar Estado", EditorStyles.boldLabel);

        if (GUILayout.Button("→ Searching"))
            ai.DebugForceState(DogAI.State.Searching);

        if (GUILayout.Button("→ Spotted"))
            ai.DebugForceState(DogAI.State.Spotted);

        if (GUILayout.Button("→ Preparar Investida"))
            ai.DebugForceState(DogAI.State.PreparingCharge);

        if (GUILayout.Button("→ Investida"))
            ai.DebugForceState(DogAI.State.Charging);

        if (GUILayout.Button("→ Cooldown"))
            ai.DebugForceState(DogAI.State.Cooldown);

        if (GUILayout.Button("→ Comer"))
            ai.DebugForceState(DogAI.State.Eating);

        if (GUILayout.Button("→ Kill"))
            ai.DebugForceState(DogAI.State.Kill);

        EditorGUILayout.EndVertical();
    }
}
