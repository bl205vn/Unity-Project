using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingSceneRoot;   // GameObject \"LoadingScene\"
    [SerializeField] private Slider loadingSlider;          // Thanh slider bên trong LoadingScene
    [SerializeField] private float fakeLoadDelay = 0.2f;    // Thời gian trễ nhỏ sau khi full
    [SerializeField] private float minLoadingTime = 2f;     // Thời gian tối thiểu để thấy loading

    private bool _isLoading;

    private void Start()
    {
        // Đảm bảo màn hình loading tắt lúc vào menu
        if (loadingSceneRoot != null)
        {
            loadingSceneRoot.SetActive(false);
        }

        // Gán các event handler cho buttons nếu chưa được gán trong Inspector
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }

    public void OnPlayClicked()
    {
        if (_isLoading)
            return;

        _isLoading = true;

        // Khóa nút để tránh bấm nhiều lần
        if (playButton != null)
            playButton.interactable = false;
        if (exitButton != null)
            exitButton.interactable = false;

        // Bật màn hình loading và bắt đầu load scene
        if (loadingSceneRoot != null)
            loadingSceneRoot.SetActive(true);

        StartCoroutine(LoadPlaySceneRoutine());
    }

    private IEnumerator LoadPlaySceneRoutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("play");

        // Không cho vào scene ngay, đợi mình cho phép
        asyncLoad.allowSceneActivation = false;

        float elapsed = 0f;
        float displayedProgress = 0f; // giá trị hiển thị trên slider (0 -> 1)

        while (!asyncLoad.isDone)
        {
            // Thời gian thực, không phụ thuộc Time.timeScale
            elapsed += Time.unscaledDeltaTime;

            // Tiến độ thật của Unity (0 -> ~0.9)
            float rawProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            bool loadFinished   = asyncLoad.progress >= 0.9f;
            bool minTimeReached = elapsed >= minLoadingTime;

            if (!loadFinished)
            {
                // Khi đang load thật: cho thanh chạy mượt tới tối đa 90%
                float targetDisplayed = Mathf.Clamp01(rawProgress) * 0.9f; // 0 -> 0.9

                displayedProgress = Mathf.MoveTowards(
                    displayedProgress,
                    targetDisplayed,
                    Time.unscaledDeltaTime * 0.7f // tốc độ chạy, tăng/giảm tùy thích
                );
            }
            else
            {
                // Đã load xong dữ liệu nhưng còn đang chờ thời gian tối thiểu
                if (!minTimeReached)
                {
                    // Giữ thanh đứng ở 90%
                    displayedProgress = 0.9f;
                }
                else
                {
                    // Hết thời gian => nhảy lên 100% và vào scene luôn
                    displayedProgress = 1f;

                    if (loadingSlider != null)
                        loadingSlider.value = displayedProgress;

                    yield return new WaitForSecondsRealtime(fakeLoadDelay);
                    asyncLoad.allowSceneActivation = true;
                    yield break;
                }
            }

            if (loadingSlider != null)
            {
                loadingSlider.value = displayedProgress;
            }

            yield return null;
        }
    }

    public void OnExitClicked()
    {
        // Thoát game
        Application.Quit();

#if UNITY_EDITOR
        // Trong Unity Editor, Application.Quit() không hoạt động
        // Nên dùng UnityEditor.EditorApplication.isPlaying = false
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        // Remove listeners để tránh memory leak
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
}

