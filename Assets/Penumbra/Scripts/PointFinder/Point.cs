using UnityEngine;

public class Point : MonoBehaviour
{
    [Header("Identifica��o")]
    public string objectName;

    [HideInInspector] public Transform selfTransform;

    private void Awake()
    {
        selfTransform = transform;
    }
}
