using UnityEngine;
using Fusion;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    public GameObject PlayerPrefab; //Khởi tạo prefab người chơi

    public void PlayerJoined(PlayerRef player)
    {
        //Kiểm tra xem người chơi có phải người chơi localPlayer không
        //LocalPlayer: Là người ở client, có toàn quyền điểu khiển
        //Proxies: Là bản sao nhân vật của các client khác, chỉ có thể nhìn thấy và tương tác với nhau
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(0, 2f, 0), Quaternion.identity, player, (runner, obj) => 
            {
                var _playerp = obj.GetComponent<PlayerSetup>();
                if (_playerp != null)
                {
                    _playerp.SetupCamera();
                }
            });
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        // Yêu cầu Câu 5: Dùng lại ChatContentText để thông báo
        ChatUI chatUI = FindAnyObjectByType<ChatUI>();
        if (chatUI != null && chatUI.chatContent != null)
        {
            chatUI.chatContent.text += "<color=red><i>Người chơi đã thoát trận.</i></color>\n";
        }
    }
}
