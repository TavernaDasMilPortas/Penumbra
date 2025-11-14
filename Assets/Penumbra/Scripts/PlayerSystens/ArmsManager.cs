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

    private void Start()
    {
        ToggleLamp(true);

        // 🔹 Garante que os braços não sejam escondidos pela câmera
        ForceEnableRenderers();
    }
    private void Update()
    {
        ForceEnableRenderers();
    }

    /// <summary>Mostra ou esconde o lampião no braço direito.</summary>
    public void ToggleLamp(bool state)
    {
        rightArm?.SetLampVisible(state);
        ForceEnableRenderers(); // garante visibilidade mesmo após mudança
    }

    /// <summary>Atualiza o item atual do braço esquerdo, vindo do inventário.</summary>
    public void EquipItem(Item newItem)
    {
        leftArm?.SetEquippedItem(newItem);
        ForceEnableRenderers();
    }

    /// <summary>Desativa ambos os braços (por exemplo, durante menus).</summary>
    public void SetArmsEnabled(bool enabled)
    {
        rightArm.enabled = enabled;
        leftArm.enabled = enabled;
    }

    /// <summary>
    /// 🔥 Garante que NENHUM MeshRenderer dos braços seja desativado por scripts ou câmeras.
    /// </summary>
    public void ForceEnableRenderers()
    {
        // 🔹 Braço esquerdo (socket do item)
        if (leftArm != null && leftArm.itemSocket != null)
        {
            EnableAllRenderers(leftArm.itemSocket);
        }

        // 🔹 Braço direito (lampião instanciado)
        if (rightArm != null && rightArm.lampSocket != null)
        {
            EnableAllRenderers(rightArm.lampSocket.transform);
        }
    }

    /// <summary>
    /// Ativa MeshRenderers e SkinnedMeshRenderers em todos os filhos do transform informado.
    /// </summary>
    private void EnableAllRenderers(Transform root)
    {
        if (root == null) return;

        var meshRenderers = root.GetComponentsInChildren<MeshRenderer>(true);
        var skinnedRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var renderer in meshRenderers)
            renderer.enabled = true;

        foreach (var renderer in skinnedRenderers)
            renderer.enabled = true;
    }
}
