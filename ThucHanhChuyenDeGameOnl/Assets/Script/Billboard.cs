using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Gắn với Camera chính trong Main Scene
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    // Dùng LateUpdate để xoay hướng sau khi camera đã di chuyển trong khung hình đó
    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Luôn hướng cái transform này (UI trên đầu) về phía Camera
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                mainCameraTransform.rotation * Vector3.up);
        }
    }
}
