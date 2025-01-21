using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class IngameUI : MonoBehaviour
{
    private VisualElement _root;
    private Label _scoreLabel;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _scoreLabel = _root.Q<Label>("scoreLabel");
        Init();
    }

    private void Init()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        _scoreLabel.text = "Score: " + SessionData.Instance.GetCurrentScore();
    }
}
