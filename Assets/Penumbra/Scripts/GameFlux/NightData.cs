using UnityEngine;

[CreateAssetMenu(fileName = "NightData", menuName = "Game/Night Data")]
public class NightData : ScriptableObject
{
    [Header("Identificação")]
    public string nightName;

    [Header("Tarefas")]
    public NightTask[] tasks;

}
