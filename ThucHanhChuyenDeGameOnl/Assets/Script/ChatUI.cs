using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("Chat UI Elements")]
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI chatContent;

    private void Start()
    {
        // Gắn sự kiện click cho nút Send
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendMessage);
        }
    }

    private void Update()
    {
        // Cho phép ấn Enter để gửi thay vì phải bấm nút bằng chuột
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }

    private void SendMessage()
    {
        string message = inputField.text;
        
        // Nếu tin nhắn không rỗng thì mới gửi
        if (!string.IsNullOrWhiteSpace(message))
        {
            if (ChatManager.Instance != null)
            {
                // Gọi hàm gửi tin qua mạng
                ChatManager.Instance.SendChatMessage(message);
                
                // Xóa ô nhập liệu sau khi đã gửi đi
                inputField.text = ""; 
                
                // Giữ lại focus (con trỏ chuột) ở ô nhập để chat tiếp cho tiện
                inputField.ActivateInputField(); 
            }
            else
            {
                Debug.LogWarning("Không tìm thấy ChatManager.Instance!");
            }
        }
    }
}
