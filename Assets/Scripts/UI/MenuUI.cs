using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    private VisualElement _root;
    private Button _newBtn, _loadBtn, _profileBtn, _achvBtn, _exitBtn;
    private Label _profileLbl;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _newBtn = _root.Q<Button>("newGameBtn");
        _loadBtn = _root.Q<Button>("loadGameBtn");
        _profileLbl = _root.Q<Label>("profileLabel");
        _profileBtn = _root.Q<Button>("profileBtn");
        _achvBtn = _root.Q<Button>("achvBtn");
        _exitBtn = _root.Q<Button>("exitGameBtn");

        if (_newBtn != null) _newBtn.clicked += NewGame;
        if (_loadBtn != null) _loadBtn.clicked += LoadGame;
        if (_profileBtn != null) _profileBtn.clicked += OpenProfile;
        if (_achvBtn != null) _achvBtn.clicked += OpenAchievement;
        if (_exitBtn != null) _exitBtn.clicked += CloseApp;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene name, LoadSceneMode mode)
    {
        UpdateProfileLabel();

        string currentProfile = DataPersistenceManager.Instance.GetSelectedProfileId();
        if (string.IsNullOrEmpty(currentProfile))
        {
            UIManager.Instance.OpenNewProfileUI();
        }
    }

    private void NewGame()
    {
        GameManager.Instance.NewGame();
    }
    private void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }

    private void OpenProfile()
    {
        UIManager.Instance.OpenProfileUI();
    }

    private void UpdateProfileLabel()
    {
        if (_profileLbl != null) _profileLbl.text = "Profile: " + DataPersistenceManager.Instance.GetSelectedProfileId();
    }

    private void OpenAchievement()
    {
        UIManager.Instance.OpenAchievementUI();
    }

    private void CloseApp()
    {
        GameManager.Instance.Quit();
    }
}
