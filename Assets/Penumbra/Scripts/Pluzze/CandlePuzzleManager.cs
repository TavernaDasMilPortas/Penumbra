using System.Collections.Generic;
using UnityEngine;

public class CandlePuzzleManager : PuzzleManager
{
    [Header("Configuração das Velas")]
    public List<Item> candleItems;

    protected override void Start()
    {
        base.Start();
        GenerateRandomOrder();
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

        // Se chegou aqui, o puzzle está correto
        foreach (var holder in holders)
        {
            holder.LockHolder(false);

            // 🔹 Procura um IInteractable dentro do holder
            var interactable = holder.GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                // Tenta desativar a interação, se possível
                if (interactable is InteractableBase baseInteractable)
                {
                    baseInteractable.IsInteractable = false;
                }

            }
        }

        OnPuzzleSolved();
    }

}
