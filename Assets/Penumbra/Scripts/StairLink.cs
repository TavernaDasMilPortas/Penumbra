using UnityEngine;
using System.Collections;

public class StairLink : MonoBehaviour
{
    [Header("Destino do teleporte")]
    public Transform exitPoint;

    [Header("Manter velocidade após teleporte?")]
    public bool keepVelocity = true;

    [Header("Collider a ser desativado (este ou o oposto)")]
    public Collider stairTrigger;   // Você arrasta o próprio collider ou o outro StairLink

    [Header("Cooldown após teleporte")]
    public float disableDuration = 0.5f; // tempo para evitar loop

    private bool isCoolingDown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCoolingDown) return; // já teleportei recentemente?
        if (!other.CompareTag("Player")) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        Vector3 currentVelocity = Vector3.zero;

        if (keepVelocity && PlayerController.Instance != null)
            currentVelocity = PlayerController.Instance.GetCurrentVelocity();

        // ---- DESATIVAR O STAIR TRIGGER PARA EVITAR LOOP ----
        if (stairTrigger != null)
        {
            StartCoroutine(DisableColliderTemp());
        }

        // ---- TELEPORTE SEGURO ----
        cc.enabled = false;
        other.transform.position = exitPoint.position;
        other.transform.rotation = exitPoint.rotation;
        cc.enabled = true;

        if (keepVelocity && PlayerController.Instance != null)
            PlayerController.Instance.SetExternalVelocity(currentVelocity);

        Debug.Log("[StairLink] Teleportado com cooldown.");
    }


    private IEnumerator DisableColliderTemp()
    {
        isCoolingDown = true;

        stairTrigger.enabled = false;

        yield return new WaitForSeconds(disableDuration);

        stairTrigger.enabled = true;

        isCoolingDown = false;
    }
}
