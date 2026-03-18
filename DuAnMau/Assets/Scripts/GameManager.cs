using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameData gameData;
    private bool canShootSpecialBullet = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        gameData.CurrentKills++;
        if (gameData.CurrentKills >= gameData.RequiredKills && !gameData.IsSpecialBulletUnlocked)
        {
            gameData.IsSpecialBulletUnlocked = true;
            canShootSpecialBullet = true;
            Debug.Log("Đạn đặc biệt đã được mở khóa!");
        }
    }

    public bool CanShootSpecialBullet()
    {
        return gameData.IsSpecialBulletUnlocked && canShootSpecialBullet;
    }

    public void OnSpecialBulletShot()
    {
        canShootSpecialBullet = false;
        gameData.ResetData();
    }

    public int GetCurrentKills()
    {
        return gameData.CurrentKills;
    }

    public int GetRequiredKills()
    {
        return gameData.RequiredKills;
    }
} 