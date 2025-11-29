using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ItemIconGenerator : MonoBehaviour
{
    [Header("Configuração da Câmera")]
    public Camera iconCamera;
    public RenderTexture renderTexture;

    [Header("Onde o item será instanciado temporariamente")]
    public Transform previewHolder;

    [Header("Itens que terão ícones gerados")]
    public List<Item> itemsToRender = new();

    [Header("Pasta para salvar os ícones (dentro de Assets/)")]
    public string saveFolder = "ItemIcons";

#if UNITY_EDITOR

    [ContextMenu("Gerar Ícones")]
    public void GenerateIcons()
    {
        if (iconCamera == null || renderTexture == null || previewHolder == null)
        {
            Debug.LogError("[ItemIconGenerator] Falta configurar a câmera, renderTexture ou previewHolder.");
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/" + saveFolder))
            AssetDatabase.CreateFolder("Assets", saveFolder);

        foreach (var item in itemsToRender)
        {
            if (item == null || item.handPrefab == null)
            {
                Debug.LogWarning($"[ItemIconGenerator] Item inválido ou sem modelo: {item}");
                continue;
            }

            GenerateSingleIcon(item);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ItemIconGenerator] Ícones gerados com sucesso!");
    }

    private void GenerateSingleIcon(Item item)
    {
        // limpa
        foreach (Transform child in previewHolder)
            DestroyImmediate(child.gameObject);

        // instancia
        GameObject inst = Instantiate(item.handPrefab, previewHolder);

        // APLICA OS OFFSETS DO ITEM
        inst.transform.localPosition = item.placementOffset;
        inst.transform.localEulerAngles = item.placementRotationOffset;
        inst.transform.localScale = item.placementScaleOffset;

        // força layer correta
        SetLayerRecursively(inst, LayerMask.NameToLayer("IconRenderer"));

        // renderiza
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // converte RT para textura 2D
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // salva PNG
        string path = $"Assets/{saveFolder}/{item.itemName}_icon.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log($"[ItemIconGenerator] Ícone salvo: {path}");

        // importa como sprite
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        importer.textureType = TextureImporterType.Sprite;
        importer.alphaIsTransparency = true;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.SaveAndReimport();

        // aplica automaticamente no item
        item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        DestroyImmediate(inst);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

#endif
}

