using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : MonoBehaviour, ICollectible
{
    public int coinValue = 5;

    [Header("Events Broadcasters")]
    public IntEvent onCoinCollected;

    public void Collect(GameObject obj)
    {
        onCoinCollected.TriggerEvent(coinValue);
        Destroy(gameObject);
        AchievementManager.Instance.ProgressAchievement("1coin", 1);
        AchievementManager.Instance.ProgressAchievement("100coins", 1);
    }
}
