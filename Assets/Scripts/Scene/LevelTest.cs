using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    int coinCount = 0;
    private void Awake()
    {
        coinCount = GetComponentsInChildren<CoinCollectible>().Count();
    }

    public void CheckCoin()
    {
        // Re-open the game scene if coin in the scene is 0
        // to refresh the coin
        coinCount--;
        if (coinCount == 0)
        {
            GameSceneManager.Instance.OpenGameScene();
        }
    }
}
