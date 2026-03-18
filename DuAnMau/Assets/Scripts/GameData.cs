using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data")]
public class GameData : ScriptableObject
{
    [SerializeField] private int requiredKills = 5;
    [SerializeField] private int currentKills = 0;
    [SerializeField] private bool isSpecialBulletUnlocked = false;

    public int RequiredKills => requiredKills;
    public int CurrentKills 
    { 
        get => currentKills;
        set => currentKills = value;
    }
    public bool IsSpecialBulletUnlocked 
    { 
        get => isSpecialBulletUnlocked;
        set => isSpecialBulletUnlocked = value;
    }

    public void ResetData()
    {
        currentKills = 0;
        isSpecialBulletUnlocked = false;
    }
} 