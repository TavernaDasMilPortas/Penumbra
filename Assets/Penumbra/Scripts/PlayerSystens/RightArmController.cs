using UnityEngine;

public class RightArmController : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;
    public GameObject lampObject;

    private bool isVisible = false;

    public void SetLampVisible(bool state)
    {
        if (lampObject == null) return;

        isVisible = state;
        lampObject.SetActive(state);
        animator?.SetBool("HoldingLamp", state);

        Debug.Log($"[RightArm] Lampião {(state ? "exibido" : "guardado")}");
    }

    private void Update()
    {
        // Teste temporário: alternar manualmente com tecla L
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetLampVisible(!isVisible);
        }
    }
}
