using UnityEngine;
using UnityEngine.InputSystem;

public class StageLightControl : MonoBehaviour
{
    [Header("Kéo đèn Spotlight vào đây (Nếu quên code sẽ tự tìm)")]
    public Light stageSpotLight;

    void Start()
    {
        // --- Code thêm: Tự động tìm đèn nếu bị null ---
        if (stageSpotLight == null)
        {
            // Tìm component Light nằm trong các con (Children) của nhân vật
            stageSpotLight = GetComponentInChildren<Light>();

            // Nếu tìm thấy thì tắt nó đi để chờ lệnh Enter
            if (stageSpotLight != null)
            {
                stageSpotLight.enabled = false;
                Debug.Log("Đã tự động tìm thấy đèn sân khấu: " + stageSpotLight.name);
            }
            else
            {
                Debug.LogError("CẢNH BÁO: Chưa có đèn Spotlight nào làm con của nhân vật!");
            }
        }
        // ----------------------------------------------
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            PerformStageEffect();
        }
    }

    void PerformStageEffect()
    {
        // 1. Tìm tất cả đèn (dùng hàm mới FindObjectsByType để tránh warning cũ)
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);

        foreach (Light l in allLights)
        {
            // Tắt tất cả đèn KHÁC đèn sân khấu
            if (l != stageSpotLight)
            {
                l.enabled = false;
            }
        }

        // 2. Bật đèn sân khấu
        if (stageSpotLight != null)
        {
            stageSpotLight.enabled = true;
            Debug.Log("Bật đèn: " + stageSpotLight.name);
        }
    }
}