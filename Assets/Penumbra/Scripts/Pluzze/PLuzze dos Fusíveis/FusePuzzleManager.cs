using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FusePuzzleManager : PuzzleManager
{
    [Header("Cores e Materiais Disponíveis")]
    public int colorCount = 3;
    public List<FuseColorMaterial> colorMaterials = new List<FuseColorMaterial>();

    [Header("ScriptableObjects de Cada Cor")]
    public List<Fuse> fuseDefinitions = new List<Fuse>();

    [Header("Sockets (definem tudo: switch, point, holder)")]
    public List<FuseSocket> sockets = new List<FuseSocket>();

    private Dictionary<FuseColor, Material> materialMap = new();
    private Dictionary<FuseColor, Fuse> fuseMap = new();

    private List<FuseColor> correctColorOrder = new();

    protected override void Start()
    {
        base.Start();

        // Mapa Material → Cor
        foreach (var cm in colorMaterials)
            if (!materialMap.ContainsKey(cm.color))
                materialMap.Add(cm.color, cm.material);

        // Mapa FuseSO → Cor
        foreach (var fuse in fuseDefinitions)
            if (!fuseMap.ContainsKey(fuse.color))
                fuseMap.Add(fuse.color, fuse);

        AssignColors();
        GenerateCorrectOrder();

        // Aplica material aos fusíveis que já foram instanciados pelo AutoItemSpawner
        ApplyMaterialsToSpawnedFuses();
    }

    // --------------------------------------------------------------------
    // 1) ATRIBUI COR + MATERIAL + SCRIPTABLE + CONFIGURA POINT
    // --------------------------------------------------------------------
    private void AssignColors()
    {
        List<FuseColor> allColors = new((FuseColor[])System.Enum.GetValues(typeof(FuseColor)));

        int usable = Mathf.Clamp(colorCount, 1, allColors.Count);
        List<FuseColor> selected = new();

        // PICK N CORES ÚNICAS
        while (selected.Count < usable)
        {
            FuseColor c = allColors[Random.Range(0, allColors.Count)];
            if (!selected.Contains(c))
                selected.Add(c);
        }

        // EMBARALHA SOCKETS
        List<FuseSocket> shuffled = new List<FuseSocket>(sockets);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int r = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[r]) = (shuffled[r], shuffled[i]);
        }

        // ATRIBUI AS CORES
        for (int i = 0; i < shuffled.Count; i++)
        {
            FuseColor chosen =
                (i < selected.Count)
                ? selected[i]
                : selected[Random.Range(0, selected.Count)];

            var socket = shuffled[i];
            socket.color = chosen;

            // ★ APLICA MATERIAL AO SWITCH
            ApplyMaterialToSwitch(socket, chosen);

            // ★ APLICA SCRIPTABLE OBJECT
            if (fuseMap.TryGetValue(chosen, out Fuse fuseSO))
            {
                socket.assignedFuse = fuseSO;
                if (socket.fusePoint != null)
                    socket.fusePoint.spawnItem = fuseSO;
            }
        }

        Debug.Log("🔧 Cores aplicadas aos sockets: " +
            string.Join(", ", sockets.ConvertAll(s => s.color.ToString())));
    }

    // --------------------------------------------------------------------
    // 2) APLICA MATERIAL AOS INTERRUPTORES
    // --------------------------------------------------------------------
    private void ApplyMaterialToSwitch(FuseSocket socket, FuseColor color)
    {
        if (!materialMap.TryGetValue(color, out var mat))
            return;

        if (socket.switchObject == null)
            return;

        Transform f1 = socket.switchObject.transform.Find("Fita");
        Transform f2 = socket.switchObject.transform.Find("Fita2");

        if (f1 != null)
        {
            Renderer r1 = f1.GetComponent<Renderer>();
            if (r1 != null) r1.material = mat;
        }

        if (f2 != null)
        {
            Renderer r2 = f2.GetComponent<Renderer>();
            if (r2 != null) r2.material = mat;
        }
    }

    // --------------------------------------------------------------------
    // 3) APLICA MATERIAL AO FUSÍVEL INSTANCIADO NO POINT
    // --------------------------------------------------------------------
    private void ApplyMaterialsToSpawnedFuses()
    {
        foreach (var socket in sockets)
        {
            if (socket.fusePoint == null) continue;
            if (socket.fusePoint.spawnItem == null) continue;

            Transform t = socket.fusePoint.transform;
            if (t.childCount == 0) continue;

            GameObject fuseModel = t.GetChild(0).gameObject;

            if (!materialMap.TryGetValue(socket.color, out var mat))
                continue;

            Renderer[] rends = fuseModel.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends)
                r.material = mat;
        }
    }

    // --------------------------------------------------------------------
    // 4) ORDEM CORRETA = ORDEM DOS SOCKETS
    // --------------------------------------------------------------------
    private void GenerateCorrectOrder()
    {
        correctColorOrder = sockets.ConvertAll(s => s.color);

        Debug.Log("🎯 ORDEM CORRETA: " +
            string.Join(", ", correctColorOrder));
    }

    // --------------------------------------------------------------------
    // 5) CHECAR SOLUÇÃO
    // --------------------------------------------------------------------
    protected override void CheckSolution()
    {
        for (int i = 0; i < holders.Count; i++)
        {
            Fuse fuse = holders[i].currentItem as Fuse;
            if (fuse == null) return;

            if (fuse.color != correctColorOrder[i])
                return;
        }

        Debug.Log("🎉 Puzzle resolvido!");
        OnPuzzleSolved();
    }
}


// --------------------------------------------------------------------
//       CLASSES DE SUPORTE
// --------------------------------------------------------------------
[System.Serializable]
public class FuseSocket
{
    public ItemHolder holder;
    public GameObject switchObject;
    public Point fusePoint;
    public FuseColor color;

    [HideInInspector] public Fuse assignedFuse;

    [HideInInspector] public Renderer fuseRenderer;
    [HideInInspector] public Renderer fitaRenderer1;
    [HideInInspector] public Renderer fitaRenderer2;
}

[System.Serializable]
public class FuseColorMaterial
{
    public FuseColor color;
    public Material material;
}

