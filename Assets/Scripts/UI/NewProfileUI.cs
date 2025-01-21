using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NewProfileUI : MonoBehaviour
{
    private VisualElement _root;
    private TextField _inputField;
    private Button _createBtn, _cancelBtn;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _inputField = _root.Q<TextField>("newProfileInput");
        _createBtn = _root.Q<Button>("createBtn");
        _cancelBtn = _root.Q<Button>("cancelBtn");

        if (_createBtn != null) _createBtn.clicked += CreateProfile;
        if (_cancelBtn != null) _cancelBtn.clicked += Cancel;

        // Disable the cancel button if no user profile was found
        // Forcing user to make new user
        string currentProfile = DataPersistenceManager.Instance.GetSelectedProfileId();
        if (string.IsNullOrEmpty(currentProfile))
        {
            _cancelBtn.SetEnabled(false);
        }
    }

    private void CreateProfile()
    {
        string profileName = _inputField.value.Trim();

        if (string.IsNullOrEmpty(profileName))
        {
            string warning = "Profile name cannot be empty.";
            UIManager.Instance.SendAlert($"{warning}");
            return;
        }

        if (!Regex.IsMatch(profileName, @"^[a-zA-Z0-9_]+$"))
        {
            string warning = "Profile name can only contain letters, numbers, and underscores.";
            UIManager.Instance.SendAlert($"{warning}");
            return;
        }

        DataPersistenceManager.Instance.CreateNewProfile(profileName);
        UIManager.Instance.CloseNewProfileUI();
        UIManager.Instance.RefreshProfileUI();
    }

    private void Cancel()
    {
        UIManager.Instance.CloseNewProfileUI();
        UIManager.Instance.RefreshProfileUI();
    }
}
