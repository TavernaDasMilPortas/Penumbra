using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public static Camera currentCamera; // muda em tempo real

    public float interactDistance = 3f;

    private void Start()
    {
        currentCamera = playerCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentCamera == null)
                currentCamera = playerCamera;

            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                SafeButton btn = hit.collider.GetComponentInParent<SafeButton>();
                if (btn != null)
                {
                    btn.Press();
                }
            }
        }
    }
}

