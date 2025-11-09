using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour
{
    [Header("Referências Comuns")]
    public List<ItemHolder> holders;

    [Header("Solução Atual e Correta")]
    protected List<Item> currentSetup = new List<Item>();
    protected List<Item> correctSetup = new List<Item>();
    protected List<Item> lastSetup = new List<Item>();

    protected bool puzzleSolved = false;

    protected virtual void Start()
    {
        currentSetup = new List<Item>(new Item[holders.Count]);
        lastSetup = new List<Item>(new Item[holders.Count]);

        foreach (var holder in holders)
        {
            currentSetup[holders.IndexOf(holder)] = holder.currentItem;
            lastSetup[holders.IndexOf(holder)] = holder.currentItem;
        }
    }

    protected virtual void Update()
    {
        if (puzzleSolved) return;

        bool changed = false;

        // Verifica se algum holder mudou de item
        for (int i = 0; i < holders.Count; i++)
        {
            var item = holders[i].currentItem;
            if (item != lastSetup[i])
            {
                lastSetup[i] = item;
                currentSetup[i] = item;
                changed = true;
            }
        }

        // Só chama verificação se houve mudança
        if (changed)
            CheckSolution();
    }

    protected abstract void CheckSolution();

    protected void OnPuzzleSolved()
    {
        puzzleSolved = true;
        Debug.Log($"✅ {name}: Puzzle resolvido!");

        // Bloqueia todos os holders
        foreach (var holder in holders)
        {
            holder.LockHolder(true);
        }
    }
}
