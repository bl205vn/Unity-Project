using UnityEngine;
using UnityEngine.InputSystem;

public class DayNightToggle : MonoBehaviour
{
    [Header("Settings")]
    public Light sunLight;

    private bool isDay = true;

    // Lưu lại giá trị ánh sáng ban đầu để khi bật ngày lên thì trả lại như cũ
    private float defaultAmbientIntensity;
    private float defaultReflectionIntensity;

    void Start()
    {
        // Lấy giá trị mặc định của scene hiện tại khi bắt đầu game
        defaultAmbientIntensity = RenderSettings.ambientIntensity;
        defaultReflectionIntensity = RenderSettings.reflectionIntensity;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleTime();
        }
    }

    void ToggleTime()
    {
        isDay = !isDay;

        if (isDay)
        {
            // --- BAN NGÀY ---
            sunLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            sunLight.intensity = 1f; // Đèn sáng lại

            // Trả lại ánh sáng môi trường và phản xạ
            RenderSettings.ambientIntensity = defaultAmbientIntensity;
            RenderSettings.reflectionIntensity = defaultReflectionIntensity;
        }
        else
        {
            // --- BAN ĐÊM (TỐI ĐEN) ---
            sunLight.transform.rotation = Quaternion.Euler(-50, -30, 0);
            sunLight.intensity = 0f; // Tắt hẳn đèn mặt trời

            // Cắt luôn ánh sáng từ bầu trời hắt xuống -> Đen thui
            RenderSettings.ambientIntensity = 0f;

            // Tắt phản xạ (để mấy vật kim loại không bị bóng sáng)
            RenderSettings.reflectionIntensity = 0f;
        }
    }
}