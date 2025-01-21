using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerControl _playerControl;
    private InputActionAsset _inputActionAsset;
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _inputActionAsset = GetComponent<PlayerInput>().actions;
            _playerActionMap = _inputActionAsset.FindActionMap("Player");
            _uiActionMap = _inputActionAsset.FindActionMap("UI");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Switch automatically between player or UI input every scene loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchActionMap();
    }

    public void Initialize(PlayerControl playerControl)
    {
        this._playerControl = playerControl;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_playerControl != null)
        {
            _playerControl.OnMove(context);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_playerControl != null)
        {
            _playerControl.OnJump(context);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (_playerControl != null)
        {
            _playerControl.OnDash(context);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.PauseGame();
        }
    }

    public void SwitchToPlayerActionMap()
    {
        _playerActionMap.Enable();
        _uiActionMap.Disable();
    }

    public void SwitchToUIActionMap()
    {
        _playerActionMap.Disable();
        _uiActionMap.Enable();
    }

    private void SwitchActionMap()
    {
        if (GameSceneManager.Instance.IsGameScene())
        {
            SwitchToPlayerActionMap();
        }
        else
        {
            SwitchToUIActionMap();
        }
    }
}
