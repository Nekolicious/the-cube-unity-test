using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementUI : MonoBehaviour
{
    private VisualElement _root;
    private ScrollView _container;
    private Button _backBtn;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _container = _root.Q<ScrollView>("achievementContainer");
        _backBtn = _root.Q<Button>("backBtn");

        _backBtn.clicked += Close;

        InitializeAchievements();
    }

    private void InitializeAchievements()
    {
        _container.Clear();

        // Fetch achievements from the AchievementsManager
        List<Achievement> achievements = AchievementManager.Instance.GetAchievements();

        foreach (Achievement achievement in achievements)
        {
            VisualElement achievementElement = new VisualElement();

            // Add locked class style if achievement is not unlocked
            if (!achievement.isUnlocked)
            {
                achievementElement.AddToClassList("locked");
            }
            else
            {
                achievementElement.AddToClassList("achievement-item");
            }

            Label nameLabel = new Label(achievement.title);
            nameLabel.AddToClassList("achievement-title");
            achievementElement.Add(nameLabel);

            Label descriptionLabel = new Label(achievement.description);
            descriptionLabel.AddToClassList("achievement-desc");
            achievementElement.Add(descriptionLabel);

            Label progressLabel = new Label($"Progress: {achievement.progress}/{achievement.target}");
            progressLabel.AddToClassList("achievement-progress");
            achievementElement.Add(progressLabel);

            _container.Add(achievementElement);
        }
    }

    private void Close()
    {
        UIManager.Instance.CloseAchievementUI();
    }
}
