using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void UnitTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnitTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    [Test]
    public void TestCalculateDamage()
    {
        //B1: khoi tao Object tu class CombatSystem (vi ham calculateDamage khai bao trong do);
        var combat = new CombatSystem();

        //AreEqual(ket qua mong muon, ket qua thuc te)
        Assert.AreEqual(15, combat.CalculateDamage(10, 1.5f));
        Assert.AreEqual(10, combat.CalculateDamage(5, 2.0f));
    }

    [Test]
    public void TestCalculateScore()
    {
        var scoreSystem = new ScoreSystem();
        Assert.AreEqual(200, scoreSystem.CalculateScore(100, 2));
        Assert.AreEqual(150, scoreSystem.CalculateScore(50, 3));
    }
}
