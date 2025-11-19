using UnityEngine;

public class BlockLightForLayer : MonoBehaviour
{
    [Header("Layer contendo os objetos que NÃO devem receber luz (somente 1 layer)")]
    public LayerMask blockedLayerMask;

    [Header("Rendering Layer usada para bloquear luz")]
    public uint renderingLayer = 1u << 30;
    // bit alto para evitar conflitos com HDRP

    [Header("Aplicar automaticamente ao iniciar?")]
    public bool applyOnStart = true;

    void Start()
    {
        if (applyOnStart)
            Apply();
    }

    [ContextMenu("Aplicar Agora")]
    public void Apply()
    {
        // ---- 1) Converter LayerMask em índice de layer ----
        int layerIndex = Mathf.RoundToInt(Mathf.Log(blockedLayerMask.value, 2));
        if (layerIndex < 0 || layerIndex > 31)
        {
            Debug.LogError("[BlockLight] LayerMask inválido! Selecione APENAS UMA layer.");
            return;
        }

        Debug.Log($"[BlockLight] Layer alvo = {LayerMask.LayerToName(layerIndex)}  (index {layerIndex})");
        Debug.Log($"[BlockLight] RenderingLayerMask = {renderingLayer}");

        // ---- 2) remover renderingLayer de TODAS as luzes ----
        Light[] allLights = FindObjectsOfType<Light>(true);
        foreach (var light in allLights)
        {
            light.renderingLayerMask &= ~(int)renderingLayer;
        }

        // ---- 3) aplicar renderingLayer aos renderers bloqueados ----
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(true);
        foreach (var r in allRenderers)
        {
            if (r.gameObject.layer == layerIndex)
            {
                r.renderingLayerMask = renderingLayer;
            }
        }

        Debug.Log("[BlockLight] Aplicação concluída — objetos na layer escolhida não receberão luz!");
    }
}
