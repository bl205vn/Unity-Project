using Unity.Cinemachine;
using UnityEngine;
using Fusion;

public class PlayerSetup : NetworkBehaviour
{
public void SetupCamera()
    {
        if (Object.HasInputAuthority)
        {
            CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
            if (cameraFollow != null)
            {
                PlayerMovement pm = GetComponent<PlayerMovement>();
                if (pm != null && pm.CinemachineCameraTarget != null)
                {
                    cameraFollow.AssignCamera(pm.CinemachineCameraTarget.transform);
                }
                else
                {
                    cameraFollow.AssignCamera(transform);
                }
            }
        }
    }
}
