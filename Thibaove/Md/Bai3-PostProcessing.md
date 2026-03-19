Nhân vật đang ở môi trường bình thường → bấm H → chuyển sang chế độ cinematic / ảo diệu:
```
Bloom (ánh sáng rực)
Tone màu ấm
Film Grain
Vignette
```
Có thể thêm Black & White để minh họa rõ sự thay đổi

## Bước 1: Tạo Global Volume
Trong Hierarchy: Right Click --> Volume --> Global Volume --> đặt tên: PostProcess_Global

Trong Inspector: Add Override:

Bloom
Color Adjustments (Unity6 dùng: White Balance)
Film Grain
Vignette

### Thiết lập:
- Bloom: 
    Intensity = 3
    Threshold = 1
- White Balance: 
    Temperature = 40
- Film Grain:
    Intensity = 0.4
- Vignette: 
    Intensity = 0.35
    Smoothness = 0.5

## Bước 2: Script bật/tắt bằng phím H
Gắn script vào PostProcess_Global.

Kéo Volume vào biến globalVolume.
Code script:
```c#
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class TogglePostProcessing : MonoBehaviour
{
    public Volume globalVolume;
    public float onWeight = 1f;
    public float offWeight = 0f;

    void Start()
    {
        if (globalVolume != null)
            globalVolume.weight = offWeight;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            if (globalVolume == null)
            {
                Debug.LogWarning("[TogglePostProcessing] Assign a Volume in the inspector.");
                return;
            }

            globalVolume.weight = Mathf.Approximately(globalVolume.weight, onWeight) ? offWeight : onWeight;
            Debug.Log($"[TogglePostProcessing] toggled weight -> {globalVolume.weight}");
        }
    }
}
``` 


## Bước 3: Bật Post Processing trong Main Camera

