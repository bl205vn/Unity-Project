using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class DamagePlayModeTest
{
    // Đóng thế (Mock) con Enemy ngay bên trong file Test giống 100% video của Giảng viên
    public class Enemy : MonoBehaviour
    {
        public int hp = 100;

        public void TakeDamage(int damage)
        {
            hp -= damage;
        }
    }

    public class Player : MonoBehaviour
    {
        public int score = 0;

        public void AddScore(int points)
        {
            score += points;
        }
    }

    [UnityTest]
    public IEnumerator ApplyDamageToEnemy()
    {
        GameObject enemyGO = new GameObject("Enemy"); 
        var enemy = enemyGO.AddComponent<Enemy>(); // Lúc này nó tự động gọi con Enemy giả ở dòng 9
        var combat = enemyGO.AddComponent<CombatSystem>();

        int damage = combat.CalculateDamage(10, 1.5f); 
        enemy.TakeDamage(damage); 

        yield return null; 
        Assert.AreEqual(85, enemy.hp); // Dùng biến hp chữ thường của con Enemy giả
    }

    [UnityTest]
    public IEnumerator UITest()
    {
        GameObject PlayerUI = new GameObject("Player");
        var player = PlayerUI.AddComponent<Player>();
        var scoreSystem = PlayerUI.AddComponent<ScoreSystem>();

        int score = scoreSystem.CalculateScore(80, 2);
        player.AddScore(score);

        yield return null;
        Assert.AreEqual(160, player.score);
    }
}