using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(PointReference))]
public class PointDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty pointNameProp = property.FindPropertyRelative("pointName");
        string currentName = pointNameProp.stringValue ?? "";

        // Tenta usar PointManager primeiro (runtime / play mode)
        string[] allNames = null;

        if (PointManager.Instance != null && PointManager.AllPointNames != null && PointManager.AllPointNames.Count > 0)
        {
            allNames = PointManager.AllPointNames.ToArray();
        }
        else
        {
            // Fallback: pega todos os Points existentes na cena (inclui inativos)
            var found = Resources.FindObjectsOfTypeAll<Point>()
                .Where(p => !EditorUtility.IsPersistent(p.gameObject) && p.gameObject.hideFlags == HideFlags.None)
                .ToArray();

            if (found != null && found.Length > 0)
                allNames = found.Select(p => p.objectName).Distinct().ToArray();
            else
                allNames = new string[0];
        }

        EditorGUI.BeginProperty(position, label, property);

        if (allNames.Length > 0)
        {
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(allNames, currentName));
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, allNames);

            if (newIndex >= 0 && newIndex < allNames.Length)
                pointNameProp.stringValue = allNames[newIndex];
        }
        else
        {
            // nenhum Point encontrado — mostra campo de texto para digitar
            EditorGUI.PropertyField(position, pointNameProp, new GUIContent(label.text + " (nenhum Point encontrado)"));
        }

        EditorGUI.EndProperty();
    }
}
