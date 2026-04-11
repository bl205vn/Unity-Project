using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    // Cài đặt enum 2 chế độ
    public enum ChatMode { Fusion, Photon }
    
    [Header("Chat UI Elements")]
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI chatContent;

    [Header("Chuyển Kênh (Tab)")]
    [Tooltip("Kéo thả nút Fusion vào đây")]
    public Button fusionTabButton;
    
    [Tooltip("Kéo thả nút Photon vào đây")]
    public Button photonTabButton;
    
    [Tooltip("Kéo thả một Text hiển thị chữ để biết đang ở Tab nào")]
    public TextMeshProUGUI currentChannelText; 

    // Mặc định lúc chơi game sẽ ở Tab Fusion
    private ChatMode currentMode = ChatMode.Fusion; 

    // Biến toàn cục (static) để các script khác (như PlayerActions, PlayerMovement) đọc được xem có đang chat không
    public static bool IsChatting = false;

    private void Start()
    {
        // Gắn sự kiện click cho các nút
        if (sendButton != null) sendButton.onClick.AddListener(SendMessage);
        
        // Cấu hình khi click vào Nút Tab Fusion/Photon thì chuyển chế độ
        if (fusionTabButton != null) fusionTabButton.onClick.AddListener(() => SwitchMode(ChatMode.Fusion));
        if (photonTabButton != null) photonTabButton.onClick.AddListener(() => SwitchMode(ChatMode.Photon));

        // Tự động setup giao diện mặc định cho dòng chữ Channel
        SwitchMode(ChatMode.Fusion);
        IsChatting = false;
    }

    private void Update()
    {
        // Xử lý phím Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (IsChatting)
            {
                // Nếu đang chat thì gửi chữ đi, sau đó đóng chat!
                SendMessage();
                SetChatModeActive(false);
            }
            else
            {
                // Nếu đang rảnh rỗi (đi lại), ấn Enter sẽ mở chat
                SetChatModeActive(true);
            }
        }

        // Nhấn phím Escape để hủy chat, quay lại điều khiển nhân vật
        if (Input.GetKeyDown(KeyCode.Escape) && IsChatting)
        {
            inputField.text = ""; // Xoá chữ gõ dở
            SetChatModeActive(false);
        }

        // --- ĐÁP ỨNG YÊU CẦU DÙNG PHÍM TẮT ĐỂ ĐỔI KÊNH ---
        // Nhấn phím F1 đổi sang Fusion
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchMode(ChatMode.Fusion);
        }
        // Nhấn phím F2 đổi sang Photon
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SwitchMode(ChatMode.Photon);
        }
    }

    // Hàm thực hiện chuyển UI sang Tab tương ứng
    private void SwitchMode(ChatMode newMode)
    {
        currentMode = newMode;
        if (currentChannelText != null)
        {
            if (newMode == ChatMode.Fusion)
                currentChannelText.text = "Đang chat kênh: <color=yellow>Fusion</color>";
            else
                currentChannelText.text = "Đang chat kênh: <color=green>Photon</color>";
        }
    }

    // Gửi Message theo từng luồng
    private void SendMessage()
    {
        string message = inputField.text;
        
        if (!string.IsNullOrWhiteSpace(message))
        {
            if (currentMode == ChatMode.Fusion)
            {
                // Nếu đang ở Tab Fusion, gọi hàm Fusion
                if (ChatManager.Instance != null)
                    ChatManager.Instance.SendChatMessage(message);
                else
                    Debug.LogWarning("Không thấy ChatManager của Fusion để gửi! Có thể chưa spawn local player.");
            }
            else if (currentMode == ChatMode.Photon)
            {
                // Nếu đang ở Tab Photon, gọi hàm Photon
                if (PhotonChatManager.Instance != null)
                    PhotonChatManager.Instance.SendChatMessage(message);
                else
                    Debug.LogWarning("Không thấy PhotonChatManager để gửi! Bạn quên kéo script ra ngoài Scene à?");
            }
            
            // Xong gửi xóa chữ
            inputField.text = ""; 
        }
    }

    // Tắt / Bật trạng thái Chat và khóa di chuyển / chuột
    private void SetChatModeActive(bool chatActive)
    {
        IsChatting = chatActive;

        // Cố gắng tìm component điều khiển của StarterAssets
        var playerInputs = FindAnyObjectByType<StarterAssets.StarterAssetsInputs>();
        if (playerInputs != null)
        {
            // Nếu đang chat: hiện chuột, khóa camera vòng quanh
            // Nếu tắt chat: khóa chuột, bật lại camera vòng quanh
            playerInputs.cursorLocked = !chatActive;
            playerInputs.cursorInputForLook = !chatActive;
            
            Cursor.lockState = chatActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = chatActive;
        }

        if (chatActive)
        {
            inputField.ActivateInputField();
        }
        else
        {
            // Tắt con trỏ gõ văn bản
            inputField.DeactivateInputField();
            // Trả quyền điều khiển phím về môi trường Unity
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
