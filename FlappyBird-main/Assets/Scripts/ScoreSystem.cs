using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int CalculateScore(int baseScore, int combo)
    {
        return baseScore * combo;
    }
}
