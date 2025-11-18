using UnityEngine;

public class SafeCameraController : MonoBehaviour
{
    public Camera safeCamera;
    public Camera mainCamera;

    public void EnterSafeView()
    {
        mainCamera.gameObject.SetActive(false);
        safeCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ButtonInteraction.currentCamera = safeCamera;
    }

    public void ExitSafeView()
    {
        safeCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ButtonInteraction.currentCamera = mainCamera;
    }
}
