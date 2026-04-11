using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public static PhotonChatManager Instance;
    private ChatClient chatClient;

    private void Awake()
    {
        // Script này chỉ cần 1 cái duy nhất trên Scene
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "asia"; // Vùng máy chủ

        // Tên ngẫu nhiên cho user
        string username = "Player_" + Random.Range(1000, 9999);
        
        // Kết nối vào ứng dụng Chat theo App ID bạn đã nhập trong PhotonServerSettings
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
    }

    private void Update()
    {
        if (chatClient != null)
        {
            // Hàm này bắt buộc gọi liên tục ở Update để giữ mạng không bị rớt
            chatClient.Service(); 
        }
    }

    public void SendChatMessage(string message)
    {
        // Nếu mạng Chat đang ổn định thì đăng tin vào phòng tên "General"
        if (chatClient != null && chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            chatClient.PublishMessage("General", message);
        }
        else
        {
            Debug.LogWarning("Photon Chat chưa kết nối!");
        }
    }

    // --- Các hàm bắt buộc tự động chạy khi kế thừa IChatClientListener ---

    public void OnConnected()
    {
        Debug.Log("Photon Chat: Đã kết nối thành công!");
        // Vừa kết nối xong thì tự động nhảy vào kênh chat chung tên là General
        chatClient.Subscribe(new string[] { "General" });
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        // Hàm này tự chạy khi kênh General có người báo tin. Ta lấy chuỗi và in ra màn hình
        for (int i = 0; i < messages.Length; i++)
        {
            string formattedMessage = $"<color=green>[Photon]</color> {senders[i]}: {messages[i]}";
            
            // TÌM UI VÀ IN RA
            ChatUI ui = FindAnyObjectByType<ChatUI>();
            if (ui != null && ui.chatContent != null)
            {
                ui.chatContent.text += formattedMessage + "\n";
            }
        }
    }

    // Các hàm phụ (Bỏ trống cũng không sao, nhưng bắt buộc phải có để báo cho máy biết đây là script Chat)
    public void DebugReturn(DebugLevel level, string message) {}
    public void OnDisconnected() {}
    public void OnChatStateChange(ChatState state) {}
    public void OnPrivateMessage(string sender, object message, string channelName) {}
    public void OnSubscribed(string[] channels, bool[] results) {}
    public void OnUnsubscribed(string[] channels) {}
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {}
    public void OnUserSubscribed(string channel, string user) {}
    public void OnUserUnsubscribed(string channel, string user) {}
}
