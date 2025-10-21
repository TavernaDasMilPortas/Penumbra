using UnityEngine;

public class Point : MonoBehaviour
{
    [Header("Identificação")]
    public string objectName;

    [HideInInspector] public Transform selfTransform;

    private void Awake()
    {
        selfTransform = transform;
    }
}
