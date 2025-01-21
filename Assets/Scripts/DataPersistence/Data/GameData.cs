using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int coinCount;
    public int gameScore;
    public Guid dataGuid;

    public GameData() {
        this.coinCount = 0;
        this.gameScore = 0;
        this.dataGuid = Guid.NewGuid();
    }
}
