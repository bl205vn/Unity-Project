using UnityEngine;

public class MoveUIElement : MonoBehaviour
{
    public RectTransform uiElement;  // UI element cần di chuyển (ví dụ: Panel)
    public Vector2 targetPosition;   // Vị trí mà UI element sẽ di chuyển đến
    public float moveSpeed = 2f;     // Tốc độ di chuyển

    void Update()
    {
        // Di chuyển UI element mượt mà từ vị trí hiện tại đến vị trí mục tiêu
        uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
    }
}
