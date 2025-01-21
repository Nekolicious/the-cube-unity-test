using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationUI : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _confirmWindow;
    private Button _yesBtn, _noBtn;
    private Action _onConfirm;
    private Action _onCancel;
    private Label _text;

    void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _confirmWindow = _root.Q<VisualElement>("confirmationWindow");
        _text = _root.Q<Label>("confirmationText");
        _yesBtn = _root.Q<Button>("yesBtn");
        _noBtn = _root.Q<Button>("noBtn");

        _yesBtn.clicked += () => ConfirmAction();
        _noBtn.clicked += () => CancelAction();
    }

    public void Show(string message, Action onConfirm, Action onCancel = null)
    {
        _text.text = message;
        _onCancel = onCancel;
        _onConfirm = onConfirm;
        _confirmWindow.style.display = DisplayStyle.Flex;
    }

    private void ConfirmAction()
    {
        _onConfirm?.Invoke();
        Hide();
    }

    private void CancelAction()
    {
        _onCancel?.Invoke();
        Hide();
    }

    private void Hide()
    {
        _confirmWindow.style.display = DisplayStyle.None;
    }
}
