using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    // Khai báo Slider để kéo thả thanh máu trên đầu vào
    public Slider hpSlider;
    public Slider mpSlider;

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
        // Phải kiểm tra xem Mạng đã gắn vào Object này chưa, nếu chưa thì bỏ qua lỗi
        if (Object == null) return;

        if (Object.HasInputAuthority)
        {
            if (Input.GetKeyDown(KeyCode.H)) HP -= 10;
            if (Input.GetKeyDown(KeyCode.J)) MP -= 5;
        }

        // Gán tiến độ cho Slider trên đầu nhân vật
        if (hpSlider != null) hpSlider.value = (float)HP / 100f;
        if (mpSlider != null) mpSlider.value = (float)MP / 50f;
    }
}
