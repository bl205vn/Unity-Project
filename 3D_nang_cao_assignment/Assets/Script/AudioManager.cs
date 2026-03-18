using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Biến Instance (Singleton) để có thể gọi AudioManager từ bất kỳ script nào
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("Dùng để phát nhạc nền (BGM)")]
    public AudioSource bgmSource;
    
    [Tooltip("Dùng để phát hiệu ứng âm thanh (SFX)")]
    public AudioSource sfxSource;

    private void Awake()
    {
        // Kiểm tra xem đã có Instance nào tồn tại chưa
        if (Instance == null)
        {
            Instance = this;
            // Giữ cho AudioManager không bị hủy khi chuyển Scene (rất quan trọng)
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có 1 AudioManager khác thì hủy cái mới sinh ra
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Hàm dùng để phát âm thanh hiệu ứng (Click, Chém, Bắn...)
    /// </summary>
    /// <param name="clip">File âm thanh muốn phát</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        // Phát âm thanh đè lên nhau mà không bị ngắt
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Hàm dùng để phát nhạc nền
    /// </summary>
    /// <param name="clip">File nhạc nền muốn phát</param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        // Nếu đang phát đúng bài này thì không phát lại từ đầu
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = true; // Nhạc nền thì lặp lại
        bgmSource.Play();
    }

    /// <summary>
    /// Dừng nhạc nền
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
