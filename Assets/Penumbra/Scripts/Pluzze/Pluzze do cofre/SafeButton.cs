using UnityEngine;

public class SafeButton : MonoBehaviour
{
    [Header("Referência ao cofre")]
    public SafeController safe;

    [Header("Número do botão")]
    [Tooltip("Defina -1 para o botão de reset")]
    public int number = 0;

    // Chamado pelo OnClick() do botão ou OnMouseDown() se quiser
    public void Press()
    {
        if (safe == null)
        {
            Debug.LogWarning($"[SafeButton] Nenhum SafeController atribuído no botão {name}!");
            return;
        }

        if (number >= 0)
        {
            // Número normal
            safe.PressButton(number);
        }
        else
        {
            // Reset
            safe.currentInput = "";
            safe.screen.UpdateScreen("");
        }
    }
}