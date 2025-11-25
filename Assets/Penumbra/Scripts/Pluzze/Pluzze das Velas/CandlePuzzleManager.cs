using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandlePuzzleManager : PuzzleManager
{
    [Header("Configuração das Velas")]
    public List<Item> candleItems;

    [Header("Acendimento das Luzes")]
    [Tooltip("Prefab ou objeto de luz da vela que deve ser encontrado dentro dos holders")]
    public GameObject candleLightPrefab;

    [Tooltip("Delay entre cada vela acendendo")]
    public float lightActivationDelay = 0.3f;

    protected override void Start()
    {
        base.Start();

        GenerateRandomOrder();

        Debug.Log("========== ORDEM DO PUZZLE ==========");

        for (int i = 0; i < holders.Count; i++)
        {
            string holderName = holders[i] != null ? holders[i].name : "(NULL)";
            string candleName = correctSetup[i] != null ? correctSetup[i].itemName : "(NULL)";

            Debug.Log($"Posição {i} → Holder: {holderName} | Vela: {candleName}");
        }

        Debug.Log("=====================================");
    }



    private void GenerateRandomOrder()
    {
        correctSetup = new List<Item>(candleItems);
        for (int i = 0; i < correctSetup.Count; i++)
        {
            int rand = Random.Range(i, correctSetup.Count);
            (correctSetup[i], correctSetup[rand]) = (correctSetup[rand], correctSetup[i]);
        }

        Debug.Log("🕯️ Ordem correta das velas:");
        for (int i = 0; i < correctSetup.Count; i++)
            Debug.Log($"Posição {i + 1}: {correctSetup[i].itemName}");
    }

    protected override void CheckSolution()
    {
        // Verifica se todas as posições estão preenchidas
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

        // Tudo certo → Puzzle resolvido
        foreach (var holder in holders)
        {
            var interactables = holder.GetComponentsInChildren<IInteractable>(true);

            foreach (var interactable in interactables)
            {
                if (interactable is InteractableBase baseInteractable && interactable != holder)
                {
                    baseInteractable.IsInteractable = false;
                    Debug.Log($"Desativou interação em {baseInteractable.gameObject.name} dentro de {holder.name}");
                }
            }
        }

        NightManager.Instance.CompleteTask("Realizar_Oferenda");

        // Acende as velas com delay
        StartCoroutine(ActivateCandleLightsSequence());

        OnPuzzleSolved();
    }

    // ===================================================================
    //  SEQUÊNCIA DE ACENDIMENTO DAS VELAS
    // ===================================================================
    private IEnumerator ActivateCandleLightsSequence()
    {
        if (candleLightPrefab == null)
        {
            Debug.LogWarning("[CandlePuzzle] Nenhum prefab de luz configurado!");
            yield break;
        }

        List<GameObject> lightsToActivate = new List<GameObject>();

        // Procura o prefab dentro de cada holder
        foreach (var holder in holders)
        {
            Transform[] children = holder.GetComponentsInChildren<Transform>(true);

            foreach (var child in children)
            {
                // Procura por nome OU por tag
                if (child.gameObject.name == candleLightPrefab.name ||
                    (!string.IsNullOrEmpty(candleLightPrefab.tag) &&
                     child.gameObject.CompareTag(candleLightPrefab.tag)))
                {
                    lightsToActivate.Add(child.gameObject);
                }
            }
        }

        if (lightsToActivate.Count == 0)
        {
            Debug.LogWarning("[CandlePuzzle] Nenhuma vela/luz encontrada nos holders!");
            yield break;
        }

        Debug.Log($"[CandlePuzzle] Ativando {lightsToActivate.Count} velas...");

        // Ativa uma por vez
        foreach (var lightObj in lightsToActivate)
        {
            lightObj.SetActive(true);
            yield return new WaitForSeconds(lightActivationDelay);
        }
    }
}
