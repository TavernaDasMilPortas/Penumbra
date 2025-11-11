using UnityEngine;

[CreateAssetMenu(fileName = "NightData", menuName = "Game/Night Data")][System.Serializable]
public class NightData : ScriptableObject
{
    [Header("Identificação")]
    public string nightName;

    [Header("Tarefas")]
    [SerializeReference, SubclassSelector(typeof(NightTask))]
    public NightTask[] tasks;

    [Header("Prefabs de Spawn da Noite")]
    public GameObject[] spawnPrefabs;
}