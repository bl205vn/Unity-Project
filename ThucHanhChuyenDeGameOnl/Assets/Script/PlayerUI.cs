using UnityEngine;
using Fusion;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI hptext;
    public TextMeshProUGUI mptext;
    private PlayerStats playerStats;

    private void Start()
    {
        // Xóa hàm Start vì lúc game vừa bật lên, mạng chưa kịp kết nối để đẻ ra Player, nên tìm ở đây sẽ bị null!
    }

    private void Update()
    {
        // Liên tục kiểm tra, nếu chưa có thì đi tìm cho đến khi thấy Player mới thôi
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        // Đã tìm thấy Player thì cập nhật chữ
        if (playerStats != null)
        {
            hptext.text = $"HP: {playerStats.HP}";
            mptext.text = $"MP: {playerStats.MP}";
        }
    }
}
