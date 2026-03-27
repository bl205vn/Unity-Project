using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera virtualCamera; //Tham chiếu đến Cinemachine Camera (Bản 3.x)

    public void AssignCamera(Transform playerTransform)
    {
        virtualCamera.Follow = playerTransform; // Gán đối tượng theo dõi cho camera
        
        // Bỏ LookAt đi để Camera lấy TRỰC TIẾP rotation từ CameraRoot thay vì cố ngước nhìn nó
        virtualCamera.LookAt = null; 
    }
}
