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
    public bool isOpen = false;

    private bool isAnimating = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [Header("Flag")]
    public bool isInteractable = true;
    public bool IsInteractable => isInteractable;

    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    private void Start()
    {
        // 🔥 Calcula rotações usando SOMENTE o eixo Y da porta (independe da orientação mundial)
        closedRotation = Quaternion.Euler(
            doorModel.localEulerAngles.x,
            closedAngle,
            doorModel.localEulerAngles.z);

        openRotation = Quaternion.Euler(
            doorModel.localEulerAngles.x,
            openAngle,
            doorModel.localEulerAngles.z);

        // 🔥 Detecta se a porta já está aberta ou fechada sem depender da rotação inicial
        float toClosed = Quaternion.Angle(doorModel.localRotation, closedRotation);
        float toOpen = Quaternion.Angle(doorModel.localRotation, openRotation);

        isOpen = toOpen < toClosed;
    }

    public void Interact()
    {
        if (!IsInteractable || isAnimating)
            return;

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
        StartCoroutine(AnimateDoor(openRotation));
    }

    private void CloseDoor()
    {
        if (isAnimating) return;
        isOpen = false;
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
