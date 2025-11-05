using UnityEngine;

public class ArmsManager : MonoBehaviour
{
    public static ArmsManager Instance { get; private set; }

    [Header("Referências")]
    public RightArmController rightArm;
    public LeftArmController leftArm;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>Mostra ou esconde o lampião no braço direito.</summary>
    public void ToggleLamp(bool state)
    {
        rightArm?.SetLampVisible(state);
    }

    /// <summary>Atualiza o item atual do braço esquerdo, vindo do inventário.</summary>
    public void EquipItem(Item newItem)
    {
        leftArm?.SetEquippedItem(newItem);
    }

    /// <summary>Desativa ambos os braços (por exemplo, durante menus).</summary>
    public void SetArmsEnabled(bool enabled)
    {
        rightArm.enabled = enabled;
        leftArm.enabled = enabled;
    }
}
