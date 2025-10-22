using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointManager))]
public class PointManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PointManager manager = (PointManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🧭 Ferramentas do Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("🔄 Refresh Points"))
        {
            manager.RefreshPointsEditor();
            Debug.Log("[PointManagerEditor] Lista de pontos atualizada no Editor!");
        }

        if (GUILayout.Button("📋 Mostrar nomes no console"))
        {
            foreach (var name in PointManager.AllPointNames)
                Debug.Log("• " + name);
        }
    }
}
