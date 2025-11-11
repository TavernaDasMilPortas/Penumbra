using System;
using UnityEngine;

/// <summary>
/// Marca um campo para exibir um seletor de subclasses no inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SubclassSelectorAttribute : PropertyAttribute
{
    public Type BaseType { get; private set; }

    public SubclassSelectorAttribute(Type baseType)
    {
        BaseType = baseType;
    }
}
