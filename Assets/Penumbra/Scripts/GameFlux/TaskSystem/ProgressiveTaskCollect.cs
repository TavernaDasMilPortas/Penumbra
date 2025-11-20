using UnityEngine;

public class ProgressiveTaskCollect : MonoBehaviour
{
    public string TaskName;

    private Collectable collectable;

    private void Awake()
    {
        collectable = GetComponent<Collectable>();
        if (collectable == null)
        {
            Debug.LogError("[ProgressiveTaskCollect] Nenhum Collectable encontrado!");
            Destroy(this);
            return;
        }

        // conecta no evento
        collectable.OnCollected += HandleCollected;
    }

    private void HandleCollected(Collectable c)
    {
        Debug.Log($"[ProgressiveTaskCollect] Item coletado → adicionando {c.collectableQuantity} à task '{TaskName}'.");

        NightManager.Instance.taskManager.AddProgress(TaskName, c.collectableQuantity);

        // remove o listener
        collectable.OnCollected -= HandleCollected;

        // destrói apenas este script
        Destroy(this);
    }
}
