using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private Type[] _subclasses;
    private string[] _names;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (SubclassSelectorAttribute)base.attribute;

        if (attribute.BaseType == null)
        {
            EditorGUI.HelpBox(position, "SubclassSelector sem tipo base definido!", MessageType.Error);
            return;
        }

        // Coleta as subclasses apenas uma vez
        if (_subclasses == null)
        {
            _subclasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => attribute.BaseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .ToArray();

            _names = _subclasses.Select(t => t.Name).ToArray();
        }

        EditorGUI.BeginProperty(position, label, property);

        // Calcula retângulos para os elementos do campo
        Rect popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        int currentIndex = -1;
        if (property.managedReferenceValue != null)
        {
            Type currentType = property.managedReferenceValue.GetType();
            currentIndex = Array.IndexOf(_subclasses, currentType);
        }

        // Dropdown para selecionar tipo
        int newIndex = EditorGUI.Popup(popupRect, "Tipo", currentIndex, _names);

        // Se o tipo mudar, cria nova instância
        if (newIndex != currentIndex && newIndex >= 0)
        {
            Type selectedType = _subclasses[newIndex];
            property.managedReferenceValue = Activator.CreateInstance(selectedType);
        }

        // Se há uma instância, desenha os campos dela
        if (property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;

            // Cria SerializedObject temporário para acessar os campos internos
            SerializedObject tempObject = new SerializedObject(property.serializedObject.targetObject);
            SerializedProperty propCopy = property.Copy();
            SerializedProperty endProp = propCopy.GetEndProperty();

            Rect nextRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            while (propCopy.NextVisible(true) && !SerializedProperty.EqualContents(propCopy, endProp))
            {
                if (propCopy.depth <= property.depth)
                    break;

                float height = EditorGUI.GetPropertyHeight(propCopy, true);
                nextRect.height = height;

                EditorGUI.PropertyField(nextRect, propCopy, true);
                nextRect.y += height + 2;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 4;

        if (property.managedReferenceValue != null)
        {
            SerializedProperty propCopy = property.Copy();
            SerializedProperty endProp = propCopy.GetEndProperty();

            while (propCopy.NextVisible(true) && !SerializedProperty.EqualContents(propCopy, endProp))
            {
                if (propCopy.depth <= property.depth)
                    break;

                height += EditorGUI.GetPropertyHeight(propCopy, true) + 2;
            }
        }

        return height;
    }
}
