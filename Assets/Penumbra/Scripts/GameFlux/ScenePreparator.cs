using UnityEngine;

public static class ScenePreparator
{
    public static void PrepareScene()
    {
        Debug.Log("🎬 Preparando cena para a próxima tentativa/noite...");

        if (NightManager.Instance == null)
        {
            Debug.LogWarning("⚠️ NightManager não encontrado — nenhum spawn instanciado!");
            return;
        }
        InstantiatePrefabs();
    }
    public static void InstantiatePrefabs()
    {
        NightData current = NightManager.Instance.CurrentNight;
        if (current.spawnPrefabs == null || current.spawnPrefabs.Length == 0)
        {
            Debug.Log("🌙 Nenhum spawn configurado para esta noite.");
            return;
        }

        // 🔹 Cria um container para manter a hierarquia organizada
        GameObject nightSpawnRoot = new GameObject($"[Spawns_{current.nightName}]");

        foreach (GameObject prefab in current.spawnPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogWarning("⚠️ Prefab nulo encontrado na lista de spawns, ignorando...");
                continue;
            }

            GameObject instance = Object.Instantiate(prefab);
            instance.transform.SetParent(nightSpawnRoot.transform);
            Debug.Log($"🌒 Instanciado prefab de spawn: {prefab.name}");
        }
    }
}
