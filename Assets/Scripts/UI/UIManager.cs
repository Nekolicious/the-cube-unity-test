using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject mainMenu;
    public GameObject gameUI;
    public GameObject pauseUI;
    public GameObject loadSaveSlotUI;
    public GameObject saveSlotUI;
    public GameObject profileUI;
    public GameObject newProfileUI;
    public NotificationUI notificationUI;
    public ConfirmationUI confirmationUI;

    public AchievementUI achievementUI;
    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameSceneManager.Instance.IsGameScene())
        {
            ShowGameUI();
        }
        else if (GameSceneManager.Instance.IsMenuScene())
        {
            ShowMenuUI();
        }
        else
        {
            GameManager.Instance.BackToMenu();
        }
    }
    private void ShowMenuUI()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        ClosePauseUI();
    }

    private void ShowGameUI()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        ClosePauseUI();
    }

    public void RefreshMenuUI()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void ClosePauseUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
            _isPaused = false;
        }
    }

    public void ShowLoadUI()
    {
        if (loadSaveSlotUI != null)
        {
            loadSaveSlotUI.SetActive(true);
        }
    }

    public void CloseLoadUI()
    {
        if (loadSaveSlotUI != null)
        {
            loadSaveSlotUI.SetActive(false);
        }
    }

    public void ShowSaveUI()
    {
        if (saveSlotUI != null)
        {
            saveSlotUI.SetActive(true);
        }
    }

    public void CloseSaveUI()
    {
        if (saveSlotUI != null)
        {
            saveSlotUI.SetActive(false);
        }
    }

    public void OpenPauseUI()
    {
        if (pauseUI != null && GameSceneManager.Instance.IsGameScene())
        {
            pauseUI.SetActive(true);
            _isPaused = true;
        }
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public void CloseProfileUI()
    {
        if (profileUI != null)
        {
            profileUI.SetActive(false);
            GameManager.Instance.BackToMenu();
        }
    }

    public void OpenProfileUI()
    {
        if (profileUI != null)
        {
            profileUI.SetActive(true);
        }
    }

    public void OpenNewProfileUI()
    {
        if (newProfileUI != null)
        {
            newProfileUI.SetActive(true);
        }
    }

    public void CloseNewProfileUI()
    {
        if (newProfileUI != null)
        {
            newProfileUI.SetActive(false);
        }
    }

    public void RefreshProfileUI()
    {
        CloseProfileUI();
        OpenProfileUI();
    }

    public void OpenAchievementUI()
    {
        if (achievementUI != null)
        {
            achievementUI.gameObject.SetActive(true);
        }
    }

    public void CloseAchievementUI()
    {
        if (achievementUI != null)
        {
            achievementUI.gameObject.SetActive(false);
        }
    }

    public void ConfirmationWindow(string text, Action onConfirm, Action onCancel = null)
    {
        if (confirmationUI != null)
        {
            confirmationUI.gameObject.SetActive(true);
            confirmationUI.Show(text, onConfirm, onCancel);
        }
    }
    public void SendAlert(string text, float duration)
    {
        notificationUI.SendAlert(text, duration);
    }

    // Send alert for 5 seconds if duration is not provided
    public void SendAlert(string text)
    {
        float duration = 5f;
        notificationUI.SendAlert(text, duration);
    }
}
