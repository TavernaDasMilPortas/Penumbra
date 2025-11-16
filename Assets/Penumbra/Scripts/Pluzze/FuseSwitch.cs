using UnityEngine;

public class FuseSwitch : MonoBehaviour
{
    [Header("Informações")]
    public string roomName;

    [Header("Cor atual do interruptor (definida pelo puzzle)")]
    public FuseColor color;

    [Header("Objeto visual do interruptor (Fita/Fita2)")]
    public GameObject switchObject;

    private void Awake()
    {
        // Se não foi atribuído no inspector, usa o próprio objeto
        if (switchObject == null)
            switchObject = this.gameObject;
    }
}
