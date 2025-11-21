using UnityEngine;
using UnityEngine.Events;

public class SafeController : MonoBehaviour
{
    [Header("Sequência correta (gerada automaticamente)")]
    public string correctCode = "";

    [Header("Sequência atual")]
    public string currentInput = "";

    [Header("Referências")]
    public SafeCameraController cameraController;
    public SafeScreenUI screen;
    public Animator doorAnimator;
    public InteractableSafe interactable;

    [Header("Collider a desativar quando o cofre abrir")]
    public Collider colliderToDisable;

    [Header("Radio Morse")]
    public RadioMorse radio;

    public UnityEvent OnSafeOpened;

    private bool isInteracting = false;
    private bool isOpened = false;

    private void Awake()
    {
        GenerateRandomCode();
    }

    private void GenerateRandomCode()
    {
        correctCode = "";

        for (int i = 0; i < 4; i++)
            correctCode += Random.Range(0, 10).ToString();

        Debug.Log($"[SafeController] Código gerado: {correctCode}");

        SendCodeToRadio(correctCode);
    }

    private void SendCodeToRadio(string code)
    {
        if (radio != null)
        {
            radio.message = code;
            Debug.Log($"[SafeController] Código enviado ao rádio: {code}");
        }
    }

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
            CheckCode();
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

            if (colliderToDisable != null)
                Destroy(colliderToDisable);

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
