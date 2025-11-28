using UnityEngine;
using UnityEngine.Events;

public class SafeController : MonoBehaviour
{
    [Header("Documento onde a senha será escrita")]
    public DocumentData documentToFill;
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
        WriteDocument();
    }

    // ============================================================
    //  GERA SENHA ALEATÓRIA
    // ============================================================
    private void GenerateRandomCode()
    {
        correctCode = "";

        for (int i = 0; i < 4; i++)
            correctCode += Random.Range(0, 10).ToString();

        Debug.Log($"[SafeController] Código gerado: {correctCode}");

        SendCodeToRadio(correctCode);
    }

    // ============================================================
    //  ENVIA SENHA PARA O RÁDIO
    // ============================================================
    private void SendCodeToRadio(string code)
    {
        if (radio != null)
        {
            radio.message = code;
            Debug.Log($"[SafeController] Código enviado ao rádio: {code}");
        }
    }

    // ============================================================
    //  ESCREVE A SENHA NO DOCUMENTO
    // ============================================================
    private void WriteDocument()
    {
        if (documentToFill == null)
        {
            Debug.LogWarning("[SafeController] Nenhum DocumentData configurado!");
            return;
        }

        if (documentToFill.pages == null || documentToFill.pages.Count == 0)
        {
            Debug.LogError("[SafeController] O DocumentData não possui páginas!");
            return;
        }

        string formattedCode = "";

        // Ex: transforma "4917" → "4 9 1 7"
        for (int i = 0; i < correctCode.Length; i++)
        {
            formattedCode += correctCode[i];
            if (i < correctCode.Length - 1)
                formattedCode += " ";
        }

        string finalText =
            "Senha do Cofre:\n" +
            formattedCode + "\n";

        documentToFill.pages[0].frontText = finalText;

        Debug.Log("[SafeController] Documento preenchido:\n" + finalText);
    }

    // ============================================================
    // INTERAÇÃO COM O COFRE
    // ============================================================
    public void StartInteraction()
    {
        if (isOpened) return;
        GameStateManager.Instance.SetState(InputState.Safe);
        isInteracting = true;
        cameraController.EnterSafeView();
        currentInput = "";
        screen.UpdateScreen(currentInput);
    }

    public void StopInteraction()
    {
        GameStateManager.Instance.RestorePreviousState();
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
