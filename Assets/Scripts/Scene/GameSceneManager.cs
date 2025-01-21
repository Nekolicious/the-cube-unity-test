using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("Scene Names")]
    public string menuScene;
    public List<string> gameScenes;

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

    public bool IsGameScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return gameScenes.Contains(activeSceneName);
    }

    public bool IsMenuScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return !string.IsNullOrEmpty(menuScene) && menuScene == activeSceneName;
    }

    public void OpenMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OpenGameScene()
    {
        SceneManager.LoadScene(gameScenes[0]);
    }

    public void OpenGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
