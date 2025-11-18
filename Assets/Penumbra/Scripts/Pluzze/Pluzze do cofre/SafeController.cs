using UnityEngine;
using UnityEngine.Events;

public class SafeController : MonoBehaviour
{
    [Header("Sequência correta")]
    public string correctCode = "5287";

    [Header("Sequência atual")]
    public string currentInput = "";

    [Header("Referências")]
    public SafeCameraController cameraController;
    public SafeScreenUI screen;
    public Animator doorAnimator;
    public InteractableSafe interactable;

    [Header("Collider a desativar quando o cofre abrir")]
    public Collider colliderToDisable;

    public UnityEvent OnSafeOpened;

    private bool isInteracting = false;
    private bool isOpened = false;

    public void StartInteraction()
    {
        if (isOpened) return;

        isInteracting = true;
        cameraController.EnterSafeView();
        currentInput = "";
        screen.UpdateScreen(currentInput);
    }

    public void StopInteraction()
    {
        cameraController.ExitSafeView();
    }

    public void PressButton(int number)
    {
        if (!isInteracting || isOpened) return;

        currentInput += number.ToString();
        screen.UpdateScreen(currentInput);

        if (currentInput.Length >= correctCode.Length)
        {
            CheckCode();
        }
    }

    private void CheckCode()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("Cofre aberto!");
            isOpened = true;

            doorAnimator.SetTrigger("Open");
            OnSafeOpened?.Invoke();
            interactable.isInteractable = false;

            // === NOVO: desativa o collider ===
            if (colliderToDisable != null)
            {
                Destroy(colliderToDisable);
                Debug.Log("[SafeController] Collider desativado após abrir o cofre.");
            }
            else
            {
                Debug.LogWarning("[SafeController] Nenhum colliderToDisable foi atribuído no Inspector!");
            }

            StopInteraction();
        }
        else
        {
            Debug.Log("Código errado!");
            currentInput = "";
            screen.UpdateScreen(currentInput);
            StopInteraction();
        }
    }
}
