using UnityEngine;

public class TimerEventReference : MonoBehaviour
{
    [Tooltip("Tempo em MM:SS")]
    public string triggerTime = "0:30";

    public string nameReference;
    [HideInInspector]
    public int triggerSecond;

    void OnValidate()
    {
        // Converte string para segundos sempre que mudar no Inspector
        triggerSecond = TimeConverter.FromString(triggerTime);
    }
}