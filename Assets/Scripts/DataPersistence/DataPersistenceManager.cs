using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.IO;

// Manage game data save and load state
public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    public static DataPersistenceManager Instance { get; private set; }

    [Header("Event Broadcaster")]
    public VoidEvent OnDataLoad;

    [Header("File Configuration")]
    [SerializeField] private string _filename;
    [SerializeField] private int _maxSaveFilePerProfile;
    [SerializeField] private int _maxProfile;

    [Header("Required Space (KB)")]
    [SerializeField] private long _requiredSpace;
    private FileDataHandler dataHandler;
    private string _selectedProfileId;

    [Header("Debug")]
    [SerializeField] private bool _initializeDataIfNull = false;
    [SerializeField] private bool _saveOnQuit = false;
    [SerializeField] private bool _useTestProfile = false;
    [SerializeField] private string _testProfileId = "test";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            this.dataHandler = new FileDataHandler(Application.persistentDataPath, _filename, _requiredSpace, _maxSaveFilePerProfile);
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            dataHandler.OnWarning += HandleDataWarning;
            dataHandler.OnError += HandleDataError;

            InitializeSelectedProfileId();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleDataWarning(string message)
    {
        UIManager.Instance.SendAlert(message);
    }

    private void HandleDataError(string message)
    {
        UIManager.Instance.SendAlert(message);
    }

    private void InitializeSelectedProfileId()
    {
        this._selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (_useTestProfile)
        {
            this._selectedProfileId = _testProfileId;
        }
    }

    /// <summary>
    /// Gets the maximum profile amount allowed to be created.
    /// </summary>
    /// <returns>
    /// Maximum profile count.
    /// </returns>
    public int GetMaxProfile()
    {
        return _maxProfile;
    }

    /// <summary>
    /// Gets the profile being used now.
    /// </summary>
    /// <returns>Profile id.</returns>
    public string GetSelectedProfileId()
    {
        return _selectedProfileId;
    }

    /// <summary>
    /// Change current used profile to desired profile.
    /// </summary>
    /// <param name="newProfileId">new profile</param>
    public void SetSelectedProfileId(string profileId)
    {
        _selectedProfileId = profileId;
    }

    /// <summary>
    /// Creates new game profile.
    /// </summary>
    /// <param name="newProfileId">New profile name.</param>
    public void CreateNewProfile(string newProfileId)
    {
        try
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, newProfileId));
            SetSelectedProfileId(newProfileId);
            NewGame();
            SaveGame(gameData.dataGuid.ToString());
        }
        catch (Exception e)
        {
            HandleDataError($"Profile cannot be created. No access or no free space? {e.Message}");
        }
    }

    /// <summary>
    /// Delete a profile.
    /// </summary>
    /// <param name="profileId">Profile to be deleted.</param>
    public void DeleteProfile(string profileId)
    {
        try
        {
            HandleDataWarning($"Deleting user profile {profileId}...");
            string profilePath = Path.Combine(Application.persistentDataPath, profileId);
            if (Directory.Exists(profilePath))
            {
                Directory.Delete(profilePath, true);
            }
            _selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        }
        catch (Exception e)
        {
            HandleDataError($"Profile cannot be deleted. No access or profile invalid? {e.Message}");
        }
    }

    /// <summary>
    /// Initiate a new game data.
    /// </summary>
    public void NewGame()
    {
        try
        {
            this.gameData = new GameData();
            foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
            {
                dataPersistenceObject.LoadGame(this.gameData);
            }
        }
        catch (Exception e)
        {
            HandleDataError($"Cannot create new game. No access or space available? {e.Message}");
        }
    }

    /// <summary>
    /// Save current data as new game file.
    /// </summary>
    public void SaveAsNewGame()
    {
        try
        {
            this.gameData = new GameData();
            foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
            {
                dataPersistenceObject.SaveGame(ref this.gameData);
            }
            dataHandler.Save(gameData, _selectedProfileId, $"0{gameData.dataGuid}");
        }
        catch (Exception e)
        {
            HandleDataError($"Cannot create new game. No access or space available? {e.Message}");
        }
    }

    /// <summary>
    /// Save current data to desired save slot.
    /// </summary>
    /// <param name="filename">Save slot to be used.</param>
    public void SaveGame(string filename)
    {
        if (gameData == null && _initializeDataIfNull)
        {
            NewGame();
        }

        if (gameData == null)
        {
            return;
        }

        // Executing all object with SaveGame interface
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveGame(ref this.gameData);
        }
        dataHandler.Save(gameData, _selectedProfileId, filename, true);
    }

    /// <summary>
    /// Load selected game data slot and trigger load game mechanism.
    /// </summary>
    /// <param name="filename">Save file to be loaded.</param>
    public void LoadGame(string filename)
    {
        this.gameData = dataHandler.Load(_selectedProfileId, filename);

        if (this.gameData == null)
        {
            return;
        }

        // Executing all object with LoadGame interface
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadGame(this.gameData);
        }

        OnDataLoad.TriggerEvent(new Void());
    }

    /// <summary>
    /// Get game data from file
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>Game data</returns>
    public GameData LoadData(string filename)
    {
        return gameData = dataHandler.Load(_selectedProfileId, filename);
    }

    /// <summary>
    /// Delete saved game by its file name
    /// </summary>
    /// <param name="profileId">User profile id.</param>
    /// <param name="fileName">Filename.</param>
    public void DeleteGameByName(string profileId, string fileName)
    {
        HandleDataWarning($"Deleting save data...");
        dataHandler.DeleteSaveFileByName(profileId, fileName);
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (_saveOnQuit) SaveGame(gameData.dataGuid.ToString());
        }
        catch (Exception e)
        {
            HandleDataWarning("[DataPersistence] There is an error when trying to save the game: " + e.Message);
        }
    }

    // Find all game components that use IDataPersistence interface.
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        HashSet<GameObject> uniqueGameObjects = new HashSet<GameObject>();
        List<IDataPersistence> uniqueDataPersistenceObjects = new List<IDataPersistence>();

        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        foreach (var dataPersistenceObject in dataPersistenceObjects)
        {
            MonoBehaviour monoBehaviour = dataPersistenceObject as MonoBehaviour;
            if (monoBehaviour != null && uniqueGameObjects.Add(monoBehaviour.gameObject))
            {
                uniqueDataPersistenceObjects.Add(dataPersistenceObject);
            }
        }

        return uniqueDataPersistenceObjects;
    }

    /// <summary>
    /// Gets all profile id.
    /// </summary>
    /// <returns>List of profiles.</returns>
    public List<string> GetAllProfiles()
    {
        return dataHandler.LoadAllProfiles();
    }

    /// <summary>
    /// Gets all saved data from desired profile
    /// </summary>
    /// <param name="profileId">User profile id.</param>
    /// <returns></returns>
    public List<string> GetAllSavedDatasFromProfile(string profileId)
    {
        return dataHandler.LoadAllSavedFiles(profileId);
    }

    /// <summary>
    /// Gets max save file count per profiles.
    /// </summary>
    /// <returns>Save file count per profile.</returns>
    public int GetMaxSaveFileCount()
    {
        return dataHandler.GetMaxSaveFileCount();
    }
}
