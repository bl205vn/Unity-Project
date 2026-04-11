using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance;
    
    [Header("UI Reference")]
    public ChatUI chatUI;

    // Tùy chọn lưu lại lịch sử ngắn của các tin nhắn (như slide 7 đã chỉ)
    private List<string> chatMessages = new List<string>();

    public override void Spawned()
    {
        base.Spawned();
        
        // Trong chế độ Shared, chỉ những object của mình đẻ ra thì mình mới có StateAuthority.
        // Gán Instance = chính nó để UI gọi gửi chat không bị nhầm sang player mạng khác.
        if (HasStateAuthority)
        {
            Instance = this;
            
            // Tự động tìm kịch bản (script) ChatUI trên hệ thống UI ở Scene
            if (chatUI == null)
            {
                chatUI = FindAnyObjectByType<ChatUI>(); // Hoặc FindObjectOfType<ChatUI>(); ở bản cũ
            }
        }
        else
        {
            // Tự động tìm UI cho các player khác trên mạng mục đích để in chữ lên cho bạn đọc
            // Vì hàm In chữ bắt buộc phải tìm được chỗ để in
            if (chatUI == null)
            {
                chatUI = FindAnyObjectByType<ChatUI>();
            }
        }
    }

    // Hàm Rpc gửi cho tất cả mọi người kể cả chính mình
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcReceiveChatMessage(string playerName, string message)
    {
        // Format lại cách hiển thị, ví dụ: "Player123: Xin chào!"
        string formattedMessage = $"<color=yellow>{playerName}</color>: {message}";
        
        // Thêm vào danh sách tạm để tra cứu nếu cần
        chatMessages.Add(formattedMessage);

        // Hiển thị ra UI thông qua ChatUI
        if (chatUI != null && chatUI.chatContent != null)
        {
            chatUI.chatContent.text += formattedMessage + "\n";
        }
    }

    public void SendChatMessage(string message)
    {
        // Lấy Id của người chơi hiện tại làm Tên (Bạn có thể sửa thành Nickname sau)
        string playerName = "Player " + Runner.LocalPlayer.PlayerId.ToString();
        
        // Gọi lệnh RPC tới hàm RpcReceiveChatMessage để nó báo cáo lên Server và trả về mọi Client
        RpcReceiveChatMessage(playerName, message);
    }
}
