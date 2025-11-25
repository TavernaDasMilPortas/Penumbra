using UnityEngine;

public class Interactable_door : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("Referências da porta")]
    public Transform doorModel;
    public float openAngle = -90f;
    public float closedAngle = 0f;
    public float openSpeed = 4f;

    [Header("Estado atual (visível no inspector)")]
    public bool isOpen = false;   // <<<< exposto agora

    private bool isAnimating = false;

    private Quaternion initialRotation;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private DoorLinkGenerator linkGen;

    [Header("Flag")]
    public bool isInteractable = true;
    public bool IsInteractable => isInteractable;

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    private void Start()
    {
        linkGen = GetComponent<DoorLinkGenerator>() ??
                  GetComponentInChildren<DoorLinkGenerator>() ??
                  GetComponentInParent<DoorLinkGenerator>();

        if (linkGen == null)
            Debug.LogWarning("[Interactable_door] Nenhum DoorLinkGenerator encontrado.");

        // 🔥 Salva a rotação exata colocada no editor
        initialRotation = doorModel.localRotation;

        // 🔥 Calcula rotações baseando-se no ponto REAL como referência
        closedRotation = initialRotation * Quaternion.Euler(0, closedAngle, 0);
        openRotation = initialRotation * Quaternion.Euler(0, openAngle, 0);

        // 🔥 NÃO ALTERA A ROTAÇÃO DA PORTA
        // Apenas sincroniza os links com o estado atual
        linkGen?.SetDoorOpen(isOpen);
    }

    public void Interact()
    {
        if (!IsInteractable || isAnimating) return;

        if (RequiredItem == null)
        {
            if (!isOpen) OpenDoor();
            else CloseDoor();
        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    private void OpenDoor()
    {
        if (isAnimating) return;

        isOpen = true;
        linkGen?.SetDoorOpen(true);

        StartCoroutine(AnimateDoor(openRotation));
    }

    private void CloseDoor()
    {
        if (isAnimating) return;

        isOpen = false;
        linkGen?.SetDoorOpen(false);

        StartCoroutine(AnimateDoor(closedRotation));
    }

    private System.Collections.IEnumerator AnimateDoor(Quaternion targetRot)
    {
        isAnimating = true;

        Quaternion startRot = doorModel.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorModel.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        doorModel.localRotation = targetRot;
        isAnimating = false;
    }
}
