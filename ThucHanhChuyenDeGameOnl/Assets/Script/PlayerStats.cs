using UnityEngine;
using Fusion;

public class PlayerStats : NetworkBehaviour
{
    [Networked] public int HP { get; set; }
    [Networked] public int MP { get; set; }
    public override void Spawned()
    {
        // Đổi Start() thành Spawned() vì biến [Networked] chỉ được phép gán giá trị sau khi hệ thống Mạng đã sinh ra nhân vật hoàn chỉnh
        if (Object.HasStateAuthority)
        {
            HP = 100;
            MP = 50;
        }
    }
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            if (Input.GetKeyDown(KeyCode.H)) HP -= 10;
            if (Input.GetKeyDown(KeyCode.J)) MP -= 5;
        }
    }
}
