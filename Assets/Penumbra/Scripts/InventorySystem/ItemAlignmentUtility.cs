using UnityEngine;

public static class ItemAlignmentUtility
{
    public static void ApplyAlignment(GameObject instance, Transform holder, Item item)
    {
        if (instance == null || holder == null || item == null)
            return;

        Debug.Log($"[ItemAlignment] Aplicando alinhamento em '{instance.name}' no holder '{holder.name}'");

        // ======================================================
        // 1) Parent igual ao holder — base fundamental
        // ======================================================
        instance.transform.SetParent(holder, false);

        // ======================================================
        // 2) Aplicar offsets locais (igual LeftArmManager)
        // ======================================================
        instance.transform.localPosition = item.placementOffset;
        instance.transform.localEulerAngles = item.placementRotationOffset;
        instance.transform.localScale = item.placementScaleOffset;

        // ======================================================
        // 3) Aplicar modos especiais simples
        // ======================================================
        switch (item.alignmentMode)
        {
            case Item.ItemAlignmentMode.Vertical:
                instance.transform.localPosition = new Vector3(
                    instance.transform.localPosition.x,
                    0f,
                    instance.transform.localPosition.z
                );
                break;

            case Item.ItemAlignmentMode.Horizontal:
                instance.transform.localPosition = new Vector3(
                    0f,
                    instance.transform.localPosition.y,
                    0f
                );
                break;
        }

        // ======================================================
        // 4) TENTAR usar AlignmentPoint como AJUSTE FINAL
        // ======================================================
        Transform alignPoint = FindDeepChild(instance.transform, item.alignmentPointName);

        if (alignPoint != null && alignPoint != instance.transform)
        {
            Debug.Log($"[ItemAlignment] Ajustando com AlignmentPoint '{alignPoint.name}'");

            // Posição do alignmentPoint em localSpace do holder
            Vector3 apWorldPos = alignPoint.position;
            Vector3 holderWorldPos = holder.position;

            // Ajuste necessário para que o alignmentPoint encoste no holder
            Vector3 worldDelta = apWorldPos - holderWorldPos;

            // Movemos o item INVERTENDO esse delta
            instance.transform.position -= worldDelta;
        }
        else
        {
            Debug.Log($"[ItemAlignment] Sem AlignmentPoint em '{instance.name}'.");
        }

        Debug.Log($"[ItemAlignment] FINAL localPos={instance.transform.localPosition}, localRot={instance.transform.localEulerAngles}, scale={instance.transform.localScale}");
    }

    // Busca profunda de children
    private static Transform FindDeepChild(Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name == name) return child;
            Transform t = FindDeepChild(child, name);
            if (t != null) return t;
        }
        return null;
    }
}
