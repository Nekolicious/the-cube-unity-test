using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manage game data used in game session.
public class SessionData : MonoBehaviour, IDataPersistence
{
    private int _gameScore = 0;
    private int _coinCount = 0;
    public static SessionData Instance { get; private set; }

    [Header("Event Broadcast")]
    public VoidEvent onScoreUpdated;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        _gameScore += score;
        ScoreIsUpdating();
    }
    public void AddCoin()
    {
        _coinCount++;
        ScoreIsUpdating();
    }
    public int GetCoinCount()
    {
        return _coinCount;
    }
    public int GetCurrentScore()
    {
        return _gameScore;
    }
    public void ScoreIsUpdating()
    {
        onScoreUpdated.TriggerEvent(new Void());
    }

    // Implement the IDataPersistence interface
    public void LoadGame(GameData data)
    {
        _coinCount = data.coinCount;
        _gameScore = data.gameScore;
        ScoreIsUpdating();
    }

    public void SaveGame(ref GameData data)
    {
        data.coinCount = _coinCount;
        data.gameScore = _gameScore;
    }
}
