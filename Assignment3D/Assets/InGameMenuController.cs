using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Quản lý menu trong scene chơi:
/// - Nhấn phím Esc (action "Momenu" trong InputSystem) để mở/đóng menu.
/// - Nút "Chơi tiếp" tiếp tục game.
/// - Nút "Thoát" thoát hẳn game (hoặc có thể sửa thành quay về MainMenu nếu muốn).
/// </summary>
public class InGameMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject menuRoot;        // GameObject chứa Canvas + 2 nút "Chơi tiếp" + "Thoát"
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    [Header("Input System")]
    [Tooltip("Kéo action 'Momenu' (Escape) từ InputSystem_Actions vào đây")]
    [SerializeField] private InputActionReference menuAction;

    [Header("Audio")]
    [Tooltip("Các AudioSource đang phát nhạc nền cần tạm dừng khi mở menu")]
    [SerializeField] private AudioSource[] musicSources;

    private bool _isMenuOpen;
    private bool[] _musicWasPlaying;   // lưu trạng thái đang phát trước khi pause

    private void Awake()
    {
        // Menu phải tắt khi mới vào scene
        SetMenu(false);

        if (musicSources != null && musicSources.Length > 0)
        {
            _musicWasPlaying = new bool[musicSources.Length];
        }
    }

    private void OnEnable()
    {
        if (menuAction != null)
        {
            menuAction.action.performed += OnMenuPerformed;
            menuAction.action.Enable();
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }

    private void OnDisable()
    {
        if (menuAction != null)
        {
            menuAction.action.performed -= OnMenuPerformed;
            menuAction.action.Disable();
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }

    private void OnMenuPerformed(InputAction.CallbackContext ctx)
    {
        // Nhấn Esc => bật/tắt menu
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        SetMenu(!_isMenuOpen);
    }

    private void SetMenu(bool open)
    {
        _isMenuOpen = open;

        if (menuRoot != null)
        {
            menuRoot.SetActive(open);
        }

        // Dừng / chạy game (vật lý, Update dùng Time.deltaTime, v.v.)
        Time.timeScale = open ? 0f : 1f;

        // Dừng / chạy nhạc
        if (open)
        {
            PauseMusic();
        }
        else
        {
            ResumeMusic();
        }

        // Chuột: mở menu thì hiện chuột, đóng menu thì khóa chuột lại cho điều khiển camera
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    private void PauseMusic()
    {
        if (musicSources == null || musicSources.Length == 0)
            return;

        // Nếu mảng lưu trạng thái chưa khởi tạo đúng kích thước thì cấp lại
        if (_musicWasPlaying == null || _musicWasPlaying.Length != musicSources.Length)
        {
            _musicWasPlaying = new bool[musicSources.Length];
        }

        for (int i = 0; i < musicSources.Length; i++)
        {
            var src = musicSources[i];
            if (src == null)
                continue;

            // Lưu lại xem trước đó source có đang phát không
            _musicWasPlaying[i] = src.isPlaying;

            if (src.isPlaying)
            {
                src.Pause(); // Pause để sau Resume phát tiếp từ chỗ dở
            }
        }
    }

    private void ResumeMusic()
    {
        if (musicSources == null || musicSources.Length == 0 || _musicWasPlaying == null)
            return;

        for (int i = 0; i < musicSources.Length && i < _musicWasPlaying.Length; i++)
        {
            var src = musicSources[i];
            if (src == null)
                continue;

            // Chỉ resume những track đang phát trước khi pause
            if (_musicWasPlaying[i])
            {
                src.UnPause();
            }
        }
    }

    public void OnContinueClicked()
    {
        SetMenu(false);
    }

    public void OnExitClicked()
    {
        // Thoát game luôn
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}


