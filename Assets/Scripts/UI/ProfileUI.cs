
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileUI : MonoBehaviour
{
    private VisualElement _root;
    private Button _cancelBtn, _newButton;
    private Label _currentProfile;
    private ScrollView _container;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _container = _root.Q<ScrollView>("profileContainer");
        _cancelBtn = _root.Q<Button>("cancelBtn");
        _newButton = _root.Q<Button>("newBtn");
        _currentProfile = _root.Q<Label>("currentProfile");

        if (_cancelBtn != null) _cancelBtn.clicked += Cancel;
        if (_newButton != null) _newButton.clicked += CreateNewProfile;

        InitializeProfileButtons();
    }

    private void InitializeProfileButtons()
    {
        _container.Clear();
        string currentProfileId = DataPersistenceManager.Instance.GetSelectedProfileId();
        _currentProfile.text = $"Current User: {currentProfileId}";

        List<string> profiles = DataPersistenceManager.Instance.GetAllProfiles();
        foreach (var profile in profiles)
        {
            string profileId = profile;

            VisualElement profileContainer = new VisualElement();
            profileContainer.AddToClassList("profile");
            _container.Add(profileContainer);

            Button btn = new Button();
            btn.text = $"{profileId}";
            btn.name = $"profileBtn_{profileId}";
            btn.AddToClassList("menuButton");
            btn.clicked += () => OnProfileButtonClick(profileId);
            profileContainer.Add(btn);

            Button deleteBtn = new Button();
            deleteBtn.text = "Delete";
            deleteBtn.name = $"deleteBtn_{profileId}";
            deleteBtn.AddToClassList("deleteButton");
            profileContainer.Add(deleteBtn);

            deleteBtn.clicked += () => OnDeleteProfileButtonClick(profileId);
        }

        if (profiles.Count >= DataPersistenceManager.Instance.GetMaxProfile())
        {
            _newButton.SetEnabled(false);
        }

        // Force user to make new profile if no user profile was found
        string currentProfile = DataPersistenceManager.Instance.GetSelectedProfileId();
        if (string.IsNullOrEmpty(currentProfile))
        {
            UIManager.Instance.OpenNewProfileUI();
        }
    }

    private void CreateNewProfile()
    {
        UIManager.Instance.OpenNewProfileUI();
        UIManager.Instance.CloseProfileUI();
    }

    private void OnProfileButtonClick(string profileId)
    {
        DataPersistenceManager.Instance.SetSelectedProfileId(profileId);
        UIManager.Instance.CloseProfileUI();
    }

    private void OnDeleteProfileButtonClick(string profileId)
    {
        UIManager.Instance.ConfirmationWindow(
            "Are you sure want to delete this user profile?",
            onConfirm: () =>
            {
                DataPersistenceManager.Instance.DeleteProfile(profileId);
                UIManager.Instance.RefreshProfileUI();
            }
        );
    }

    private void Cancel()
    {
        UIManager.Instance.CloseProfileUI();
    }
}
