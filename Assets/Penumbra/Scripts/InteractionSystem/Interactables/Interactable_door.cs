using UnityEngine;

public class Interactable_door : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("Referências da porta")]
    public Transform doorModel;           // O objeto que irá girar
    public float openAngle = -90f;        // Ângulo final quando aberta
    public float closedAngle = 0f;        // Ângulo final quando fechada
    public float openSpeed = 4f;          // Velocidade de abertura/fechamento

    private bool isOpen = false;
    private bool isAnimating = false;     // Evita sobreposição de animações
    private DoorLinkGenerator linkGen;

    [Header("Flag")]
    public bool isInteractable = true;
    public bool IsInteractable => isInteractable;

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    private void Start()
    {
        // Procura o DoorLinkGenerator
        linkGen = GetComponent<DoorLinkGenerator>();
        if (linkGen == null) linkGen = GetComponentInChildren<DoorLinkGenerator>();
        if (linkGen == null) linkGen = GetComponentInParent<DoorLinkGenerator>();

        if (linkGen == null)
            Debug.LogWarning("[Interactable_door] Nenhum DoorLinkGenerator encontrado.");

        // Garante que a porta começa fechada
        if (doorModel != null)
            doorModel.localRotation = Quaternion.Euler(0, closedAngle, 0);
    }

    public void Interact()
    {
        if (!IsInteractable || isAnimating)
            return;

        if (RequiredItem == null)
        {
            if (!isOpen)
                OpenDoor();
            else
                CloseDoor();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    // ===============================
    //        MÉTODO DE ABRIR
    // ===============================
    private void OpenDoor()
    {
        if (isAnimating) return;

        isOpen = true;
        if (linkGen != null)
            linkGen.SetDoorOpen(true);

        StartCoroutine(AnimateDoor(openAngle));
    }

    // ===============================
    //        MÉTODO DE FECHAR
    // ===============================
    private void CloseDoor()
    {
        if (isAnimating) return;

        isOpen = false;
        if (linkGen != null)
            linkGen.SetDoorOpen(false);

        StartCoroutine(AnimateDoor(closedAngle));
    }

    // ===============================
    //      ANIMAÇÃO DA PORTA
    // ===============================
    private System.Collections.IEnumerator AnimateDoor(float targetY)
    {
        isAnimating = true;

        Quaternion startRot = doorModel.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetY, 0);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorModel.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        doorModel.localRotation = endRot;
        isAnimating = false;

        Debug.Log(isOpen ? "Porta aberta" : "Porta fechada");
        Debug.Log(InteractionMessage);
    }
}
