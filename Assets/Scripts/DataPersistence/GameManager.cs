using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Master game manager to manage game state
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
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

    private void Start()
    {
        // Set an achievement for first time opening the game
        AchievementManager.Instance.ProgressAchievement("welcome", 1);
    }
    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
        GameSceneManager.Instance.OpenGameScene();
    }
    public void LoadGame()
    {
        UIManager.Instance.ShowLoadUI();
    }

    public void SaveGame()
    {
        UIManager.Instance.ShowSaveUI();
    }

    public void LoadAndPlayGame(string filename)
    {
        DataPersistenceManager.Instance.LoadGame(filename);
        GameSceneManager.Instance.OpenGameScene();
    }

    public void PauseGame()
    {
        if (UIManager.Instance.IsPaused())
        {
            UIManager.Instance.ClosePauseUI();
            InputManager.Instance.SwitchToPlayerActionMap();
        }
        else
        {
            UIManager.Instance.OpenPauseUI();
            InputManager.Instance.SwitchToUIActionMap();
        }
    }

    public void BackToMenu()
    {
        GameSceneManager.Instance.OpenMenuScene();
        UIManager.Instance.RefreshMenuUI();
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Other game related method
    public void PlayerOnHighestPoint()
    {
        AchievementManager.Instance.ProgressAchievement("climber", 1);
    }
}
