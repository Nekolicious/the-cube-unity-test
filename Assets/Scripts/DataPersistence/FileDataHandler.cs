using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

public class FileDataHandler
{
    private string _dataFilePath;
    private string _dataFileName;
    private long _requiredSpace;
    private int _maxSaveFilesPerProfile;
    public event Action<string> OnWarning;
    public event Action<string> OnError;

    public FileDataHandler(string dataFilePath, string dataFileName, long requiredSpace, int maxSaveFilesPerProfile)
    {
        this._dataFileName = dataFileName;
        this._dataFilePath = dataFilePath;
        this._requiredSpace = requiredSpace;
        this._maxSaveFilesPerProfile = maxSaveFilesPerProfile;
    }

    /// <summary>
    /// Get the path of saved file game
    /// </summary>
    /// <param name="profileId">User profile id.</param>
    /// <param name="filename">File name.</param>
    /// <param name="dontUseExtension">Include the file extension?</param>
    /// <returns></returns>
    private string GetSavePathByFileName(string profileId, string filename, bool dontUseExtension = true)
    {
        if (!dontUseExtension) return Path.Combine(_dataFilePath, profileId, $"{filename}.bin");
        else return Path.Combine(_dataFilePath, profileId, $"{filename}");
    }

    public int GetMaxSaveFileCount()
    {
        return _maxSaveFilesPerProfile;
    }

    /// <summary>
    /// Load save data file from user directory.
    /// </summary>
    /// <param name="profileId">Current profile in use.</param>
    /// <param name="guid">Save slot to be loaded.</param>
    /// <returns>GameData to be used.</returns>
    public GameData Load(string profileId, string filename)
    {
        if (profileId == null)
        {
            return null;
        }

        // string fullPath = GetSaveFilePath(profileId, guid);
        string fullPath = GetSavePathByFileName(profileId, filename);
        GameData data = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load game data in encrypted Binary format (AES)
                using (FileStream fs = new(fullPath, FileMode.Open))
                {
                    byte[] key = new byte[32];
                    byte[] iv = new byte[16];
                    fs.Read(key, 0, key.Length);
                    fs.Read(iv, 0, iv.Length);

                    using (Aes aes = Aes.Create())
                    {
                        using (CryptoStream cs = new(fs, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                        {
                            BinaryFormatter formatter = new();
                            data = (GameData)formatter.Deserialize(cs);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Unable to load game data, data may corrupted or invalid. {e.Message}");
            }
        }
        return data;
    }

    /// <summary>
    /// Save game data as file and store to user directory.
    /// </summary>
    /// <param name="data">Game data to be saved.</param>
    /// <param name="profileId">Profile directory.</param>
    /// <param name="filename">Save slot to be used.</param>
    public void Save(GameData data, string profileId, string filename, bool overwriteMode = false)
    {
        // string fullPath = GetSaveFilePath(profileId, filename);
        string fullPath = GetSavePathByFileName(profileId, filename, overwriteMode);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Check if there is enough free space before saving the game
            DriveInfo driveInfo = new(Path.GetPathRoot(fullPath));
            if (driveInfo.AvailableFreeSpace > (_requiredSpace * _requiredSpace))
            {
                // Save game data in encrypted Binary format (AES)
                using (FileStream fs = new(fullPath, FileMode.Create))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] key = aes.Key;
                        byte[] iv = aes.IV;

                        fs.Write(key, 0, key.Length);
                        fs.Write(iv, 0, iv.Length);

                        using (CryptoStream cs = new(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            BinaryFormatter formatter = new();
                            formatter.Serialize(cs, data);
                        }
                    }
                }
            }
            else
            {
                OnError?.Invoke("Not enough free space to save game.");
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Failed to save game. No access? {e.Message}");
        }
    }

    /// <summary>
    /// Delete saved game file by its filename
    /// </summary>
    /// <param name="profileId">User profile id.</param>
    /// <param name="filename">File name</param>
    public void DeleteSaveFileByName(string profileId, string filename)
    {
        string fullPath = GetSavePathByFileName(profileId, filename);
        try
        {
            // Do not delete anything if path is invalid.
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                OnWarning?.Invoke($"Unable to delete save file, no data was found at: {fullPath}");
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Failed to delete save data. Data is invalid? {e.Message}");
        }
    }

    /// <summary>
    /// Loads all profile names.
    /// </summary>
    /// <returns>List of string of profile.</returns>
    public List<string> LoadAllProfiles()
    {
        List<string> profileList = new List<string>();

        // Get all directory name
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataFilePath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            profileList.Add(profileId);
        }

        return profileList;
    }

    /// <summary>
    /// Loads all saved data file from user profile.
    /// </summary>
    /// <param name="profileId">Profile id.</param>
    /// <returns>String of file name game data.</returns>
    public List<string> LoadAllSavedFiles(string profileId)
    {
        List<string> validFiles = new List<string>();
        DirectoryInfo profilePath = new DirectoryInfo(Path.Combine(_dataFilePath, profileId));
        FileInfo[] files = profilePath.GetFiles($"*.bin");
        Array.Sort(files, (x, y) => DateTime.Compare(x.CreationTime, y.CreationTime));

        foreach (FileInfo file in files)
        {
            string fileName = file.Name;
            validFiles.Add(fileName);
        }

        return validFiles;
    }

    /// <summary>
    /// Gets recently updated profile.
    /// </summary>
    /// <returns>Profile name or null</returns>
    public string GetMostRecentlyUpdatedProfileId()
    {
        string recentProfileId = null;
        DateTime lastLatest = new DateTime(2000, 1, 1);
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataFilePath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            if (dirInfo.LastWriteTime > lastLatest)
            {
                recentProfileId = dirInfo.Name;
                lastLatest = dirInfo.LastWriteTime;
            }
        }
        return recentProfileId;
    }
}
