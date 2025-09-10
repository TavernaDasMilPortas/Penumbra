using UnityEngine;

public class FacingTarget : MonoBehaviour
{
    [HideInInspector] public bool isVisible;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetVisible(bool state)
    {
        isVisible = state;

        // Exemplo simples: muda cor para vermelho quando visível
        if (rend != null)
            rend.material.color = state ? Color.red : Color.white;
    }
}
