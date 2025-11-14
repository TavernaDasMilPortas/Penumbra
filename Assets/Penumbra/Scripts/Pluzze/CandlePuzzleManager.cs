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
            //holder.LockHolder(false);

            // Pega todos os IInteractable dentro do holder, inclusive filhos inativos
            var interactables = holder.GetComponentsInChildren<IInteractable>(true);

            foreach (var interactable in interactables)
            {
                // Evita afetar o próprio holder se ele também implementar IInteractable
                if (interactable is InteractableBase baseInteractable && interactable != holder)
                {
                    baseInteractable.IsInteractable = false;
                    Debug.Log($"Desativou interação em {baseInteractable.gameObject.name} dentro de {holder.name}");
                }
            }
        }


        OnPuzzleSolved();
    }

}
