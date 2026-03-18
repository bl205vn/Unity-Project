using Unity.Cinemachine;
using UnityEngine;

public class ChuyenCam : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camGoc1;
    [SerializeField] private CinemachineCamera camGoc3;

    private const int PriorityThap = 1;
    private const int PriorityCao = 100;

    private bool isFirstPerson = false;

    private void Start()
    {
        SetCameraView(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleCamera();
        }
    }

    private void ToggleCamera()
    {
        isFirstPerson = !isFirstPerson;
        SetCameraView(isFirstPerson);
    }

    private void SetCameraView(bool firstPerson)
    {
        if (camGoc1 == null || camGoc3 == null) return;

        if (firstPerson)
        {
            camGoc1.Priority = PriorityCao;
            camGoc3.Priority = PriorityThap;
            Debug.Log("Đã chuyển sang Góc nhìn thứ nhất (Priority: 100)");
        }
        else
        {
            camGoc1.Priority = PriorityThap;
            camGoc3.Priority = PriorityCao;
            Debug.Log("Đã chuyển sang Góc nhìn thứ ba (Priority: 100)");
        }
    }
}