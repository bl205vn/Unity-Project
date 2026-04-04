using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 2: Test Âm thanh - Tiếng vỗ cánh của chim
/// Logic: Khi chim nhảy (flap), âm thanh vỗ cánh phải được phát ra
/// </summary>
public class TestAudio_BirdFlapSound
{
    [Test]
    public void Audio_BirdFlap_AudioSource_TonTai()
    {
        // 1. Setup: Tạo Bird với AudioSource
        GameObject birdObj = new GameObject("Bird");
        AudioSource audioSource = birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();

        // 2. Assert: AudioSource component phải được gắn trên Bird
        Assert.IsNotNull(audioSource, "Bird phải có component AudioSource để phát âm thanh vỗ cánh.");
    }

    [Test]
    public void Audio_BirdFlap_BirdFly_DuocGan()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird");
        AudioSource audioSource = birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        bird.birdfly = audioSource; // Gán AudioSource cho birdfly

        // 2. Assert: Biến birdfly phải được gán và không null
        Assert.IsNotNull(bird.birdfly, "Biến birdfly của FlappyBirdScript phải được gán AudioSource để phát âm thanh khi nhảy.");
    }

    [Test]
    public void Audio_BirdFlap_AudioSource_KhongTuDongPlay()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird");
        AudioSource audioSource = birdObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 2. Assert: AudioSource phải được cấu hình không tự động phát khi Awake
        Assert.IsFalse(audioSource.playOnAwake, "Âm thanh vỗ cánh không được tự phát khi game mới bắt đầu, chỉ phát khi nhấn Space.");
        Assert.IsNull(audioSource.clip, "AudioSource không được có clip mặc định - chỉ phát khi có tương tác.");
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name != "Main Camera")
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
