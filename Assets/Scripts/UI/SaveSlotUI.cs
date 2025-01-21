using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveSlotUI : MonoBehaviour
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
        InitializeSaveButtons();

        if (_cancelBtn != null) _cancelBtn.clicked += Cancel;
    }

    private void OnDeleteButtonClick(string fileName)
    {
        UIManager.Instance.ConfirmationWindow("Are you sure want to delete this data?",
        onConfirm: () =>
        {
            string profile = DataPersistenceManager.Instance.GetSelectedProfileId();
            DataPersistenceManager.Instance.DeleteGameByName(profile, fileName);
            InitializeSaveButtons();
        });
    }

    private void InitializeSaveButtons()
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
        int saveSlotCount = fileDatas.Count;
        if (fileDatas == null)
        {
            OnCreateButtonClick();
        }
        else
        {
            int i = 1;
            foreach (string file in fileDatas)
            {
                GameData data = DataPersistenceManager.Instance.LoadData(file);
                VisualElement profileContainer = new VisualElement();
                profileContainer.AddToClassList("profile");

                Button button = new Button();
                button.text = $"#{i} Score: {data.gameScore}";
                button.name = $"saveBtn_{i}";
                button.AddToClassList("menuButton");
                button.clicked += () => OnCreateButtonClick($"{file}");
                profileContainer.Add(button);

                Button deleteBtn = new Button();
                deleteBtn.text = "Delete";
                deleteBtn.name = $"deleteBtn_{i}";
                deleteBtn.AddToClassList("deleteButton");
                deleteBtn.clicked += () => OnDeleteButtonClick(file);
                profileContainer.Add(deleteBtn);

                _saveSlotContainer.Add(profileContainer);
                i++;
            }
            // If the number of save slots is less than the maximum allowed, add a button to create a new save
            if (saveSlotCount < DataPersistenceManager.Instance.GetMaxSaveFileCount())
            {
                CreateNewSaveButton();
            }
        }


    }

    private void OnCreateButtonClick(string filename = null)
    {
        if (filename == null)
        {
            DataPersistenceManager.Instance.SaveAsNewGame();
        }
        else
        {
            DataPersistenceManager.Instance.SaveGame(filename);
        }
        Cancel();
    }

    private void CreateNewSaveButton()
    {
        Button button = new Button();

        button.text = $"Create New Save";
        button.name = $"saveBtn";
        button.AddToClassList("menuButton");

        button.clicked += () => OnCreateButtonClick();

        _saveSlotContainer.Add(button);
    }

    private void Cancel()
    {
        UIManager.Instance.CloseSaveUI();
    }

    private void UpdateProfileLabel()
    {
        if (_profileLabel != null) _profileLabel.text = "User: " + DataPersistenceManager.Instance.GetSelectedProfileId();
    }
}
