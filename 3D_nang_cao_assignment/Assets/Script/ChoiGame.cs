using UnityEngine;

public class ChoiGame : MonoBehaviour
{
    [Header("Giao diện UI")]
    [Tooltip("Kéo GameObject Panel chứa màn hình Menu (có nút Play) vào đây")]
    public GameObject menuUI;

    [Header("Thành phần trong Game")]
    [Tooltip("Kéo các GameObject Gameplay (Player, Enemy...) vào danh sách này")]
    public GameObject[] gamePlayObjects;

    [Header("Âm thanh")]
    [Tooltip("Kéo AudioSource phát nhạc nền ở Menu vào đây (nếu có)")]
    public AudioSource nhacNenMenu;
    [Tooltip("Kéo AudioSource phát âm thanh khi bấm nút Play vào đây (nếu có)")]
    public AudioSource amThanhNutBam;

    void Start()
    {
        // Khi bắt đầu, hiện UI Menu và ẩn Gameplay
        if (menuUI != null) menuUI.SetActive(true);
        
        foreach (GameObject obj in gamePlayObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        // Bật nhạc nền Menu nếu có
        if (nhacNenMenu != null && !nhacNenMenu.isPlaying)
        {
            nhacNenMenu.Play();
        }

        // Đóng băng thời gian trong game khi đang ở màn hình chờ (để quái vật và vật lý không hoạt động)
        Time.timeScale = 0f;

        // Mở khóa chuột và hiện chuột để có thể bấm nút (Rất quan trọng khi dùng StarterAssets)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Hàm này sẽ dùng để gắn vào sự kiện On Click () của nút Button "Chơi Game"
    public void StartGame()
    {
        // Phát âm thanh khi bấm nút (nếu có)
        if (amThanhNutBam != null)
        {
            amThanhNutBam.Play();
        }

        // Ẩn UI Menu đi
        if (menuUI != null) menuUI.SetActive(false);

        // Tắt nhạc nền của Menu khi vào game
        if (nhacNenMenu != null)
        {
            nhacNenMenu.Stop();
        }

        // Bật các thành phần Gameplay lên
        foreach (GameObject obj in gamePlayObjects)
        {
            if (obj != null) obj.SetActive(true);
        }

        // Cho thời gian chạy bình thường trở lại (1f là tốc độ gốc)
        Time.timeScale = 1f;

        // Khóa chuột lại và ẩn chuột đi để chơi game chuẩn StarterAssets
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Hàm này sẽ dùng để gắn vào sự kiện On Click () của nút Button "Thoát Game"
    public void ThoatGame()
    {
        // Phát tiếng click giống nút Play (nếu muốn có thể tạo biến tiếng click khác cũng đc)
        if (amThanhNutBam != null)
        {
            amThanhNutBam.Play();
        }

        Debug.Log("Đang tắt game...");

        // Lệnh này xài khi đã Build ra file báo cáo đính kèm (.exe/.apk)
        Application.Quit();

#if UNITY_EDITOR
        // Lệnh này dùng để dừng thử nghiệm ngay trong cửa sổ Unity Editor 
        // (chứ hàm Application.Quit() không có tác dụng trong Editor)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
