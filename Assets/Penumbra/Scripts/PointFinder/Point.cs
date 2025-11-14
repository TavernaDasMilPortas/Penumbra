using UnityEngine;

public class Point : MonoBehaviour
{
    [Header("Identificação")]
    public string objectName;

    [Header("Opcional - Item a spawnar aqui ao iniciar a cena")]
    public Item spawnItem; // se preenchido, AutoItemSpawner criará a instância aqui

    [HideInInspector] public Transform selfTransform;

    private void Awake()
    {
        selfTransform = transform;
    }
}
