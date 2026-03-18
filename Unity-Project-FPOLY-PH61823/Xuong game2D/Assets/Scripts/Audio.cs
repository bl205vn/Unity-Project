using UnityEngine;
using System.Collections;

public class PlayLoseSound : MonoBehaviour
{
    public AudioClip loseSound;  // Âm thanh thua
    private AudioSource audioSource;

    void Start()
    {
        // Lấy AudioSource từ GameObject
        audioSource = GetComponent<AudioSource>();

        // Đảm bảo âm thanh được lặp lại
        audioSource.loop = true;

        // Bắt đầu Coroutine để phát âm thanh sau 2 giây
        StartCoroutine(PlaySoundWithDelay());
    }

    // Coroutine để trì hoãn âm thanh
    private IEnumerator PlaySoundWithDelay()
    {
        // Đợi 2 giây
        yield return new WaitForSeconds(2f);

        // Phát âm thanh "You Lose" và lặp lại
        if (loseSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(loseSound);
        }
    }
}
