using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int CalculateDamage(int baseDamage, float multiplier)
    {
        return Mathf.FloorToInt(baseDamage * multiplier);
    }
}
