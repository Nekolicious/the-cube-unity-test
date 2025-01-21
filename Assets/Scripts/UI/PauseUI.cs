using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    private VisualElement root;
    private Button _resumeBtn, _saveBtn, _loadBtn, _exitBtn;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        _resumeBtn = root.Q<Button>("resumeBtn");
        _saveBtn = root.Q<Button>("saveBtn");
        _loadBtn = root.Q<Button>("loadBtn");
        _exitBtn = root.Q<Button>("exitBtn");

        if (_resumeBtn != null) _resumeBtn.clicked += Resume;
        if (_saveBtn != null) _saveBtn.clicked += Save;
        if (_loadBtn != null) _loadBtn.clicked += Load;
        if (_exitBtn != null) _exitBtn.clicked += ExitToMenu;
    }

    private void Resume()
    {
        GameManager.Instance.PauseGame();
    }

    private void Save()
    {
        GameManager.Instance.SaveGame();
    }

    private void Load()
    {
        GameManager.Instance.LoadGame();
    }

    private void ExitToMenu()
    {
        GameManager.Instance.BackToMenu();
    }
}
