using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI hptext;
    public TextMeshProUGUI mptext;
    
    // Khai báo thêm biến Slider (Nhớ kéo thả Slider vào Inspector nhé)
    public Slider hpSlider;
    public Slider mpSlider;
    
    private PlayerStats playerStats;

    private void Start()
    {
        // Xóa hàm Start vì lúc game vừa bật lên, mạng chưa kịp kết nối để đẻ ra Player, nên tìm ở đây sẽ bị null!
    }

    private void Update()
    {
        // Liên tục kiểm tra, nếu chưa có thì đi tìm cho đến khi thấy LOCAL Player mới thôi
        if (playerStats == null)
        {
            // Trong game Online, phải tìm đúng nhân vật của MÌNH (có InputAuthority)
            PlayerStats[] allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
            foreach (var p in allPlayers)
            {
                if (p.Object != null && p.Object.HasInputAuthority)
                {
                    playerStats = p;
                    break;
                }
            }
        }

        // Đã tìm thấy Local Player thì cập nhật chữ và thanh ở góc màn hình
        if (playerStats != null)
        {
            if (hptext != null) hptext.text = $"HP: {playerStats.HP}";
            if (mptext != null) mptext.text = $"MP: {playerStats.MP}";

            // Gán giá trị cho Slider (chia cho Max HP/MP để ra số từ 0 - 1)
            // Lưu ý: Tui giả định Max HP bên PlayerStats là 100, và Max MP là 50
            if (hpSlider != null) hpSlider.value = (float)playerStats.HP / 100f;
            if (mpSlider != null) mpSlider.value = (float)playerStats.MP / 50f;
        }
    }
}
