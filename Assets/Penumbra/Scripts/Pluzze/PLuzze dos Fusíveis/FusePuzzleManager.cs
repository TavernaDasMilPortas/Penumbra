    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class FusePuzzleManager : PuzzleManager
    {
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
            SpawnFuses();
        }

    // ============================================================
    // 1️⃣ CRIA ORDEM ALEATÓRIA DE CORES PARA OS SOCKETS
    // ============================================================
    private void AssignColors()
    {
        // Todas as cores possíveis
        List<FuseColor> allColors = new((FuseColor[])System.Enum.GetValues(typeof(FuseColor)));

        // Limita para não extrapolar o total de cores existentes
        int usableColors = Mathf.Clamp(colorCount, 1, allColors.Count);

        // 1️⃣ Sortear usableColors cores únicas
        List<FuseColor> selectedPalette = new();
        while (selectedPalette.Count < usableColors)
        {
            FuseColor c = allColors[Random.Range(0, allColors.Count)];
            if (!selectedPalette.Contains(c))
                selectedPalette.Add(c);
        }

        // 2️⃣ Lista de sockets que ainda precisam ser preenchidos
        List<FuseSocket> freeSockets = new List<FuseSocket>(sockets);

        // 3️⃣ Primeiro, garantir que cada cor apareça pelo menos uma vez
        foreach (var color in selectedPalette)
        {
            if (freeSockets.Count == 0) break;

            int index = Random.Range(0, freeSockets.Count);
            FuseSocket socket = freeSockets[index];
            freeSockets.RemoveAt(index);

            ApplyColorToSocket(socket, color);
        }

        // 4️⃣ Preencher os sockets restantes com cores aleatórias do palette
        foreach (var socket in freeSockets)
        {
            FuseColor chosen = selectedPalette[Random.Range(0, selectedPalette.Count)];
            ApplyColorToSocket(socket, chosen);
        }

        Debug.Log("🎨 Cores sorteadas (palette): " + string.Join(", ", selectedPalette));
    }

    // Função auxiliar para evitar repetir código
    private void ApplyColorToSocket(FuseSocket socket, FuseColor color)
    {
        socket.color = color;

        if (fuseMap.TryGetValue(color, out Fuse fuseSO))
            socket.assignedFuse = fuseSO;

        ApplyMaterialToSwitch(socket, color);
    }



    // ============================================================
    // 2️⃣ ENVIA PARA O AUTO ITEM SPAWNER
    // ============================================================
    private void SpawnFuses()
        {
            List<Item> itemsToSpawn = new();

            foreach (var socket in sockets)
            {
                if (socket.assignedFuse == null)
                    continue;

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

        // ============================================================
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

    // ============================================================
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
