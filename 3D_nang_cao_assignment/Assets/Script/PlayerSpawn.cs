using UnityEngine;
using Fusion;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab; //Khởi tạo prefab người chơi

    public void PlayerJoined(PlayerRef player)
    {
        //Kiểm tra xem người chơi có phải người chơi localPlayer không
        //LocalPlayer: Là người ở client, có toàn quyền điểu khiển
        //Proxies: Là bản sao nhân vật của các client khác, chỉ có thể nhìn thấy và tương tác với nhau
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }
}
