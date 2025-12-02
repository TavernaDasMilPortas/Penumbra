using UnityEngine;

public class InputDebugger : MonoBehaviour
{
    public Camera cam;
    void Update()
    {
        if (cam == null) cam = Camera.main;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[INPUTDBG] Key E pressed");
            Ray r = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            if (Physics.Raycast(r, out RaycastHit hit, 10f))
            {
                Debug.Log($"[INPUTDBG] Raycast hit: {hit.collider.gameObject.name} layer={LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            }
            else
            {
                Debug.Log("[INPUTDBG] Raycast didn't hit anything");
            }
        }
    }
}
