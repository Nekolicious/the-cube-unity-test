using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadSaveSlotUI : MonoBehaviour
{
    private VisualElement _root;
    private Button _cancelBtn;
    private ScrollView _saveSlotContainer;
    private Label _profileLabel;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _cancelBtn = _root.Q<Button>("cancelBtn");
        _profileLabel = _root.Q<Label>("profileLabel");
        _saveSlotContainer = _root.Q<ScrollView>("saveSlotContainer");

        UpdateProfileLabel();
        InitializeLoadButtons();

        if (_cancelBtn != null) _cancelBtn.clicked += Cancel;
    }

    private void InitializeLoadButtons()
    {
        _saveSlotContainer.Clear();

        // Get the current profile ID
        string currentProfileId = DataPersistenceManager.Instance.GetSelectedProfileId();
        if (string.IsNullOrEmpty(currentProfileId))
        {
            return;
        }

        // Get the saved files for the current profile
        List<string> fileDatas = DataPersistenceManager.Instance.GetAllSavedDatasFromProfile(currentProfileId);
        if (fileDatas == null)
        {
            Label label = new Label();
            label.text = "No saved data found.";
            label.AddToClassList("mutedText");

            _saveSlotContainer.Add(label);
            return;
        }

        int savedDataCount = fileDatas.Count;

        // Extra protection
        if (savedDataCount == 0)
        {
            Label label = new Label();
            label.text = "No data was found.";
            label.AddToClassList("mutedText");

            _saveSlotContainer.Add(label);
            return;
        }

        // Load all game data as interactable button
        int i = 1;
        foreach (string filename in fileDatas)
        {
            GameData data = DataPersistenceManager.Instance.LoadData(filename);
            VisualElement profileContainer = new VisualElement();
            profileContainer.AddToClassList("profile");
            _saveSlotContainer.Add(profileContainer);

            Button button = new Button();
            button.text = $"#{i} Score: {data.gameScore}";
            button.name = $"loadBtn{i}";
            button.AddToClassList("menuButton");
            button.clicked += () => OnLoadButtonClick(currentProfileId, filename);
            profileContainer.Add(button);
            i++;
        }
    }

    private void OnLoadButtonClick(string profileId, string filename)
    {
        GameManager.Instance.LoadAndPlayGame(filename);
        Cancel();
    }

    private void Cancel()
    {
        UIManager.Instance.CloseLoadUI();
    }

    private void UpdateProfileLabel()
    {
        if (_profileLabel != null) _profileLabel.text = "User: " + DataPersistenceManager.Instance.GetSelectedProfileId();
    }
}