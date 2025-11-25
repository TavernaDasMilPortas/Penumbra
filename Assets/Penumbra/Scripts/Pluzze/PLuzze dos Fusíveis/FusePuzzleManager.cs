using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FusePuzzleManager : PuzzleManager
{
    [Header("Night necessária para ativar o puzzle")]
    public NightData requiredNight;

    [Header("Documento onde será escrita a ordem dos fusíveis")]
    public DocumentData documentToFill; // <<< NOVO

    [Header("Cores e Materiais Disponíveis")]
    public int colorCount;
    public List<FuseColorMaterial> colorMaterials = new();

    [Header("ScriptableObjects de Cada Cor")]
    public List<Fuse> fuseDefinitions = new();

    [Header("Sockets (cada fusível: switch, holder)")]
    public List<FuseSocket> sockets = new();

    [Header("Grupo usado para spawn dos fusíveis")]
    public PointGroup fuseSpawnGroup;

    private Dictionary<FuseColor, Material> materialMap = new();
    private Dictionary<FuseColor, Fuse> fuseMap = new();

    private List<FuseColor> correctColorOrder = new();

    protected override void Start()
    {
        base.Start();

        // --------------------------------------------------------------
        // 1 — VERIFICA NOITE
        // --------------------------------------------------------------
        if (requiredNight != null)
        {
            if (NightManager.Instance == null)
            {
                Debug.LogError("[FusePuzzle] NightManager não encontrado.");
                return;
            }

            if (NightManager.Instance.CurrentNight != requiredNight)
            {
                Debug.Log($"[FusePuzzle] Puzzle ignorado — noite atual é '{NightManager.Instance.CurrentNight.nightName}', mas pertence à '{requiredNight.nightName}'.");
                return;
            }
        }

        // --------------------------------------------------------------
        // 2 — CONFIGURAÇÃO NORMAL DO PUZZLE
        // --------------------------------------------------------------

        if (fuseSpawnGroup == null)
        {
            Debug.LogError("[FusePuzzle] Nenhum PointGroup conectado!");
            return;
        }

        foreach (var cm in colorMaterials)
            materialMap.TryAdd(cm.color, cm.material);

        foreach (var f in fuseDefinitions)
            fuseMap.TryAdd(f.color, f);

        AssignColors();
        GenerateCorrectOrder();

        // NOVO — escreve documento
        WriteDocument();

        SpawnFuses();
    }

    // ======================================================================
    //  GERA PALETA E APLICA CORES NOS SOCKETS
    // ======================================================================
    private void AssignColors()
    {
        List<FuseColor> allColors = new((FuseColor[])System.Enum.GetValues(typeof(FuseColor)));
        int usableColors = Mathf.Clamp(colorCount, 1, allColors.Count);

        List<FuseColor> selectedPalette = new();
        while (selectedPalette.Count < usableColors)
        {
            FuseColor c = allColors[Random.Range(0, allColors.Count)];
            if (!selectedPalette.Contains(c))
                selectedPalette.Add(c);
        }

        List<FuseSocket> freeSockets = new List<FuseSocket>(sockets);

        foreach (var color in selectedPalette)
        {
            if (freeSockets.Count == 0) break;

            int index = Random.Range(0, freeSockets.Count);
            FuseSocket socket = freeSockets[index];
            freeSockets.RemoveAt(index);

            ApplyColorToSocket(socket, color);
        }

        foreach (var socket in freeSockets)
        {
            FuseColor chosen = selectedPalette[Random.Range(0, selectedPalette.Count)];
            ApplyColorToSocket(socket, chosen);
        }

        Debug.Log("🎨 Cores sorteadas (palette): " + string.Join(", ", selectedPalette));
    }

    private void ApplyColorToSocket(FuseSocket socket, FuseColor color)
    {
        socket.color = color;

        if (fuseMap.TryGetValue(color, out Fuse fuseSO))
            socket.assignedFuse = fuseSO;

        ApplyMaterialToSwitch(socket, color);
    }

    // ======================================================================
    //  ESCREVE DOCUMENTO
    // ======================================================================
    private void WriteDocument()
    {
        if (documentToFill == null)
        {
            Debug.LogWarning("[FusePuzzle] Nenhum DocumentData configurado.");
            return;
        }

        if (documentToFill.pages == null || documentToFill.pages.Count == 0)
        {
            Debug.LogError("[FusePuzzle] DocumentData não possui páginas.");
            return;
        }

        string title = "Alinhamento dos Fusíveis\n\n";
        string body = "";

        for (int i = 0; i < correctColorOrder.Count; i++)
        {
            FuseColor c = correctColorOrder[i];

            string fuseName = fuseMap.ContainsKey(c) ? fuseMap[c].itemName : c.ToString();
            body += $"{i + 1}º — {fuseName}\n";
        }

        string finalText = title + body;

        documentToFill.pages[0].frontText = finalText;

        Debug.Log("[FusePuzzle] Documento preenchido com a ordem dos fusíveis:\n" + finalText);
    }

    // ======================================================================
    private void SpawnFuses()
    {
        List<Item> itemsToSpawn = new();

        foreach (var socket in sockets)
        {
            if (socket.assignedFuse != null)
                itemsToSpawn.Add(socket.assignedFuse);
        }

        AutoItemSpawner spawner = FindObjectOfType<AutoItemSpawner>();

        if (spawner == null)
        {
            Debug.LogError("[FusePuzzle] AutoItemSpawner não encontrado!");
            return;
        }

        spawner.SpawnGroupItems(fuseSpawnGroup.groupName, itemsToSpawn);
    }

    private void ApplyMaterialToSwitch(FuseSocket socket, FuseColor color)
    {
        if (!materialMap.TryGetValue(color, out Material mat)) return;
        if (socket.switchObject == null) return;

        var f1 = socket.switchObject.transform.Find("Fita");
        var f2 = socket.switchObject.transform.Find("Fita2");

        if (f1) { var r = f1.GetComponent<Renderer>(); if (r) r.material = mat; }
        if (f2) { var r = f2.GetComponent<Renderer>(); if (r) r.material = mat; }
    }

    private void GenerateCorrectOrder()
    {
        correctColorOrder = sockets.ConvertAll(s => s.color);
    }

    protected override void CheckSolution()
    {
        for (int i = 0; i < sockets.Count; i++)
        {
            Fuse fuse = holders[i].currentItem as Fuse;
            if (fuse == null) return;
            if (fuse.color != correctColorOrder[i]) return;
        }

        Debug.Log("🎉 Puzzle resolvido!");
        OnPuzzleSolved();
    }
}

[System.Serializable]
public class FuseSocket
{
    public ItemHolder holder;
    public GameObject switchObject;

    public FuseColor color;
    [HideInInspector] public Fuse assignedFuse;
}

[System.Serializable]
public class FuseColorMaterial
{
    public FuseColor color;
    public Material material;
}
