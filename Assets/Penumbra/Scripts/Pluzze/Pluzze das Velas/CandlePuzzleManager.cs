using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandlePuzzleManager : PuzzleManager
{
    [Header("Configuração das Velas")]
    public List<Item> candleItems;

    [Header("Documento onde a ordem será escrita")]
    public DocumentData documentToFill;

    [Header("Acendimento das Luzes")]
    public GameObject candleLightPrefab;
    public float lightActivationDelay = 0.3f;

    protected override void Start()
    {
        base.Start();

        GenerateRandomOrder();
        WriteDocument();
        LogDebugOrder();
    }

    // ============================================================
    // 1) GERA ORDEM ALEATÓRIA DAS VELAS
    // ============================================================
    private void GenerateRandomOrder()
    {
        correctSetup = new List<Item>(candleItems);
        for (int i = 0; i < correctSetup.Count; i++)
        {
            int rand = Random.Range(i, correctSetup.Count);
            (correctSetup[i], correctSetup[rand]) = (correctSetup[rand], correctSetup[i]);
        }
    }

    // ============================================================
    // 2) ESCREVE A ORDEM DAS VELAS NO DOCUMENTO
    // ============================================================
    private void WriteDocument()
    {
        if (documentToFill == null)
        {
            Debug.LogWarning("[CandlePuzzle] Nenhum DocumentData foi atribuído!");
            return;
        }

        if (documentToFill.pages == null || documentToFill.pages.Count == 0)
        {
            Debug.LogError("[CandlePuzzle] O DocumentData não possui páginas!");
            return;
        }

        // Monta o texto
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Altar para os que já foram\n");

        for (int i = 0; i < correctSetup.Count; i++)
        {
            string candleName = correctSetup[i] != null ? correctSetup[i].itemName : "(Vazio)";
            sb.AppendLine($"{i + 1}º - {candleName}");
        }

        // Escreve na primeira página
        documentToFill.pages[0].frontText = sb.ToString();

        Debug.Log("[CandlePuzzle] Documento preenchido:\n" + sb.ToString());
    }

    // ============================================================
    // (Opcional) DEBUG VISUAL NA CONSOLE
    // ============================================================
    private void LogDebugOrder()
    {
        Debug.Log("========== ORDEM DO PUZZLE ==========");

        for (int i = 0; i < holders.Count; i++)
        {
            string holderName = holders[i] != null ? holders[i].name : "(NULL)";
            string candleName = correctSetup[i] != null ? correctSetup[i].itemName : "(NULL)";
            Debug.Log($"Posição {i} → Holder: {holderName} | Vela: {candleName}");
        }

        Debug.Log("=====================================");
    }

    // ============================================================
    // RESTO DO CÓDIGO DO PUZZLE (não alterado)
    // ============================================================
    protected override void CheckSolution()
    {
        for (int i = 0; i < holders.Count; i++)
        {
            if (currentSetup[i] == null)
                return;

            if (currentSetup[i] != correctSetup[i])
                return;
        }

        foreach (var holder in holders)
        {
            var interactables = holder.GetComponentsInChildren<IInteractable>(true);
            foreach (var interactable in interactables)
            {
                if (interactable is InteractableBase baseInteractable && interactable != holder)
                    baseInteractable.IsInteractable = false;
            }
        }

        NightManager.Instance.CompleteTask("Realizar_Oferenda");
        StartCoroutine(ActivateCandleLightsSequence());
        OnPuzzleSolved();
    }

    private IEnumerator ActivateCandleLightsSequence()
    {
        if (candleLightPrefab == null) yield break;

        List<GameObject> lights = new List<GameObject>();

        foreach (var holder in holders)
        {
            Transform[] children = holder.GetComponentsInChildren<Transform>(true);

            foreach (var child in children)
            {
                if (child.gameObject.name == candleLightPrefab.name ||
                    (!string.IsNullOrEmpty(candleLightPrefab.tag) &&
                     child.CompareTag(candleLightPrefab.tag)))
                {
                    lights.Add(child.gameObject);
                }
            }
        }

        foreach (var light in lights)
        {
            light.SetActive(true);
            yield return new WaitForSeconds(lightActivationDelay);
        }
    }
}
