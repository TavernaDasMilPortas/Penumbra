using UnityEngine;

public class WeaponCamFollower : MonoBehaviour
{
    public Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Weapon cam sempre replica a main cam no fim do frame
        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}
