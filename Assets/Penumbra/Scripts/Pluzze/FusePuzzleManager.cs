using System.Collections.Generic;
using UnityEngine;

public class FusePuzzleManager : PuzzleManager
{
    [Header("Configuração de Cores e Materiais")]
    [Tooltip("Quantidade de cores diferentes que serão usadas neste puzzle.")]
    [Range(1, 5)]
    public int colorCount = 3;

    [Tooltip("Materiais disponíveis para cada cor de fusível.")]
    public List<FuseColorMaterial> colorMaterials = new List<FuseColorMaterial>();

    [Header("Sockets dos Fusíveis")]
    public List<FuseSocket> sockets = new List<FuseSocket>();

    private Dictionary<FuseColor, Material> materialMap = new();

    protected override void Start()
    {
        base.Start();

        // Cria mapa de materiais por cor
        foreach (var cm in colorMaterials)
        {
            if (!materialMap.ContainsKey(cm.color))
                materialMap.Add(cm.color, cm.material);
        }

        // Configura cada socket com cor e materiais
        AssignColorsToSockets();
        GenerateCorrectSetup();
    }

    private void AssignColorsToSockets()
    {
        List<FuseColor> allColors = new List<FuseColor>(
            (FuseColor[])System.Enum.GetValues(typeof(FuseColor))
        );

        // Limita ao número de cores disponíveis
        int usableColors = Mathf.Clamp(colorCount, 1, allColors.Count);

        // Escolhe N cores aleatórias únicas
        List<FuseColor> selectedColors = new List<FuseColor>();
        while (selectedColors.Count < usableColors)
        {
            FuseColor randomColor = allColors[Random.Range(0, allColors.Count)];
            if (!selectedColors.Contains(randomColor))
                selectedColors.Add(randomColor);
        }

        // 🔹 Embaralha a lista de sockets para distribuir de forma aleatória
        List<FuseSocket> shuffledSockets = new List<FuseSocket>(sockets);
        for (int i = 0; i < shuffledSockets.Count; i++)
        {
            int rand = Random.Range(i, shuffledSockets.Count);
            (shuffledSockets[i], shuffledSockets[rand]) = (shuffledSockets[rand], shuffledSockets[i]);
        }

        // 🔹 Garante que cada cor apareça ao menos uma vez
        int colorIndex = 0;
        for (int i = 0; i < shuffledSockets.Count; i++)
        {
            FuseColor chosenColor;

            // Nas primeiras N posições, use uma de cada cor
            if (i < selectedColors.Count)
            {
                chosenColor = selectedColors[i];
            }
            else
            {
                // Depois disso, repita aleatoriamente entre as cores disponíveis
                chosenColor = selectedColors[Random.Range(0, selectedColors.Count)];
            }

            FuseSocket socket = shuffledSockets[i];
            socket.color = chosenColor;

            ApplyMaterialToSocket(socket, chosenColor);
        }

        Debug.Log($"🔧 Distribuição final de fusíveis: {string.Join(", ", shuffledSockets.ConvertAll(s => s.color.ToString()))}");
    }


    private void ApplyMaterialToSocket(FuseSocket socket, FuseColor color)
    {
        if (!materialMap.TryGetValue(color, out Material mat))
        {
            Debug.LogWarning($"[FusePuzzle] Material não encontrado para cor {color}");
            return;
        }

        // Fusível visual (se existir)
        if (socket.fuseVisual != null)
        {
            Renderer fuseRenderer = socket.fuseVisual.GetComponentInChildren<Renderer>();
            if (fuseRenderer != null)
                fuseRenderer.material = mat;
            socket.fuseRenderer = fuseRenderer;
        }

        // Interruptor (fita e fita2)
        if (socket.switchObject != null)
        {
            Transform fita1 = socket.switchObject.transform.Find("Fita");
            Transform fita2 = socket.switchObject.transform.Find("Fita2");

            if (fita1 != null)
            {
                Renderer r1 = fita1.GetComponent<Renderer>();
                if (r1 != null) r1.material = mat;
                socket.fitaRenderer1 = r1;
            }

            if (fita2 != null)
            {
                Renderer r2 = fita2.GetComponent<Renderer>();
                if (r2 != null) r2.material = mat;
                socket.fitaRenderer2 = r2;
            }
        }
    }

    private void GenerateCorrectSetup()
    {
        correctSetup = new List<Item>();
        foreach (var socket in sockets)
        {
            if (socket.holder != null && socket.holder.currentItem != null)
                correctSetup.Add(socket.holder.currentItem);
            else
                correctSetup.Add(null);
        }

        // Embaralha a ordem correta
        for (int i = 0; i < correctSetup.Count; i++)
        {
            int rand = Random.Range(i, correctSetup.Count);
            (correctSetup[i], correctSetup[rand]) = (correctSetup[rand], correctSetup[i]);
        }

        Debug.Log("🔌 Ordem correta de fusíveis definida.");
    }

    protected override void CheckSolution()
    {
        // Verifica se todos os holders estão preenchidos
        for (int i = 0; i < holders.Count; i++)
        {
            if (currentSetup[i] == null)
                return;
        }

        // Compara com a ordem correta
        for (int i = 0; i < holders.Count; i++)
        {
            if (currentSetup[i] != correctSetup[i])
                return;
        }

        // Se chegou aqui, puzzle resolvido!
        foreach (var holder in holders)
        {
            //holder.LockHolder(false);

            var interactables = holder.GetComponentsInChildren<IInteractable>(true);
            foreach (var interactable in interactables)
            {
                if (interactable is InteractableBase baseInteractable && interactable != holder)
                {
                    baseInteractable.IsInteractable = false;
                    Debug.Log($"[FusePuzzle] Desativou interação em {baseInteractable.gameObject.name}");
                }
            }
        }

        OnPuzzleSolved();
    }
}

[System.Serializable]
public class FuseColorMaterial
{
    public FuseColor color;
    public Material material;
}

[System.Serializable]
public class FuseSocket
{
    public ItemHolder holder;
    public GameObject fuseVisual;
    public GameObject switchObject;
    public FuseColor color;

    [HideInInspector] public Renderer fuseRenderer;
    [HideInInspector] public Renderer fitaRenderer1;
    [HideInInspector] public Renderer fitaRenderer2;
}
