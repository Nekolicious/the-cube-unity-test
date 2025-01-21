using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public bool isUnlocked;
    public int progress;
    public int target;

    public void Unlock()
    {
        isUnlocked = true;
    }

    public bool CheckProgress(int value)
    {
        if (!isUnlocked)
        {
            progress += value;
            if (progress >= target)
            {
                Unlock();
                return true;
            }
        }
        return false;
    }
}
