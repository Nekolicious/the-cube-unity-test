using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    public List<AchievementData> achievementDefinitions;
    [SerializeField] private string achievementFileName;
    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadAchievements();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAchievements()
    {
        foreach (var data in achievementDefinitions)
        {
            if (!achievements.ContainsKey(data.id))
            {
                achievements[data.id] = new Achievement
                {
                    id = data.id,
                    title = data.title,
                    description = data.description,
                    target = data.target,
                    progress = 0,
                    isUnlocked = false
                };
            }
        }
    }

    public void ProgressAchievement(string id, int value)
    {
        if (achievements.TryGetValue(id, out var achievement))
        {
            bool unlocked = achievement.CheckProgress(value);
            if (unlocked)
            {
                NotifyAchievementUnlocked(achievement);
            }
        }
    }

    private void NotifyAchievementUnlocked(Achievement achievement)
    {
        UIManager.Instance.SendAlert($"Achievement Unlocked! [{achievement.title}]");
        SaveAchievements();
    }

    private string AchievementPath()
    {
        return Path.Combine(Application.persistentDataPath, achievementFileName);
    }

    public void SaveAchievements()
    {
        List<Achievement> achievementsList = new List<Achievement>(achievements.Values);
        string json = JsonUtility.ToJson(new AchievementListWrapper { achievements = achievementsList });
        File.WriteAllText(AchievementPath(), json);
    }

    public void LoadAchievements()
    {
        string path = AchievementPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var loadedData = JsonUtility.FromJson<AchievementListWrapper>(json);
            foreach (var loadedAchievement in loadedData.achievements)
            {
                achievements[loadedAchievement.id] = loadedAchievement;
            }
        }
        InitializeAchievements();
    }

    public List<Achievement> GetAchievements()
    {
        return new List<Achievement>(achievements.Values);
    }


    [System.Serializable]
    private class AchievementListWrapper
    {
        public List<Achievement> achievements;
    }
}
