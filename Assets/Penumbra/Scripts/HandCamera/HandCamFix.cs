using UnityEngine;

[ExecuteAlways]
public class HandCamFix : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Mantém o near plane extremamente próximo sem quebrar o HDRP
        float n = 0.0001f;
        float f = cam.farClipPlane;

        cam.nearClipPlane = n;

        Matrix4x4 p = Matrix4x4.Perspective(
            cam.fieldOfView,
            cam.aspect,
            n,
            f
        );

        cam.projectionMatrix = p;
    }
}
